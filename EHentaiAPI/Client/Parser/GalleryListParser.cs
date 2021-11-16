﻿using AngleSharp.Dom;
using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.ExtendFunction;
using EHentaiAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EHentaiAPI.Client.Parser
{
    public class GalleryListParser
    {

        private const String TAG = nameof(GalleryListParser);

        private static readonly Regex PATTERN_RATING = new Regex("\\d+px");
        private static readonly Regex PATTERN_THUMB_SIZE = new Regex("height:(\\d+)px;width:(\\d+)px");
        private static readonly Regex PATTERN_FAVORITE_SLOT = new Regex("background-color:rgba\\((\\d+),(\\d+),(\\d+),");
        private static readonly Regex PATTERN_PAGES = new Regex("(\\d+) page");
        private static readonly Regex PATTERN_NEXT_PAGE = new Regex("page=(\\d+)");

        private static readonly String[][] FAVORITE_SLOT_RGB = new String[][]{
            new String[]{"0", "0", "0"},
            new String[]{"240", "0", "0"},
            new String[] { "240", "160", "0" },
            new String[] { "208", "208", "0" },
            new String[] { "0", "128", "0" },
            new String[] { "144", "240", "64" },
            new String[] { "64", "176", "240" },
            new String[] { "0", "0", "240" },
            new String[] { "80", "0", "128" },
            new String[] { "224", "128", "224" },
    };

        private static int parsePages(Utils.Document d, String body)
        {
            try
            {
                var es = d.getElementsByClass("ptt").FirstOrDefault().Children[(0)].Children[(0)].Children;
                return int.Parse(es[(es.Length - 2)].Text().Trim());
            }
            catch (Exception)
            {
                //ExceptionUtils.throwIfFatal(e);
                throw new ParseException("Can't parse gallery list pages", body);
            }
        }

        private static String parseRating(String ratingStyle)
        {
            var m = PATTERN_RATING.Match(ratingStyle);
            int num1 = int.MinValue;
            int num2 = int.MinValue;
            int rate = 5;
            String re;
            if (m.Success)
            {
                num1 = ParserUtils.parseInt(m.Groups[(0)].Value.Replace("px", ""), int.MinValue);
            }
            if (m.Success)
            {
                num2 = ParserUtils.parseInt(m.Groups[(0)].Value.Replace("px", ""), int.MinValue);
            }
            if (num1 == int.MinValue || num2 == int.MinValue)
            {
                return null;
            }
            rate = rate - num1 / 16;
            if (num2 == 21)
            {
                rate--;
                re = rate.ToString();
                re = re + ".5";
            }
            else
                re = rate.ToString();
            return re;
        }

        private static int parseFavoriteSlot(String style)
        {
            var m = PATTERN_FAVORITE_SLOT.Match(style);
            if (m.Success)
            {
                String r = m.Groups[(1)].Value;
                String g = m.Groups[(2)].Value;
                String b = m.Groups[(3)].Value;
                int slot = 0;
                foreach (String[] rgb in FAVORITE_SLOT_RGB)
                {
                    if (r.Equals(rgb[0]) && g.Equals(rgb[1]) && b.Equals(rgb[2]))
                    {
                        return slot;
                    }
                    slot++;
                }
            }
            return -2;
        }

        private static GalleryInfo parseGalleryInfo(Settings settings, IElement e)
        {
            GalleryInfo gi = new GalleryInfo();

            // Title, gid, token (required), tags
            var glname = e.GetElementsByClassName("glname").FirstOrDefault();
            if (glname != null)
            {
                var a = glname.GetElementsByTagName("a").FirstOrDefault();
                if (a == null)
                {
                    var parent = glname.ParentElement;
                    if (parent != null && "a".Equals(parent.TagName))
                    {
                        a = parent;
                    }
                }
                if (a != null)
                {
                    GalleryDetailUrlParser.Result result = GalleryDetailUrlParser.parse(a.GetAttribute("href"));
                    if (result != null)
                    {
                        gi.gid = result.gid;
                        gi.token = result.token;
                    }
                }

                var Children = glname;
                var children = glname.Children;
                while (children.Length != 0)
                {
                    Children = children[(0)];
                    children = Children.Children;
                }
                gi.title = Children.Text().Trim();

                var tbody = glname.GetElementsByTagName("tbody").FirstOrDefault();
                if (tbody != null)
                {
                    List<String> tags = new();
                    GalleryTagGroup[] groups = GalleryDetailParser.parseTagGroups(tbody.Children);
                    foreach (GalleryTagGroup Groups in groups)
                    {
                        for (int j = 0; j < Groups.size(); j++)
                        {
                            tags.Add(Groups.groupName + ":" + Groups.getTagAt(j));
                        }
                    }
                    gi.simpleTags = tags.ToArray();
                }
            }
            if (gi.title == null)
            {
                return null;
            }

            // Category
            gi.category = EhUtils.UNKNOWN;
            var ce = e.GetElementByIdRecursive("cn");
            if (ce == null)
            {
                ce = e.GetElementsByClassName("cs").FirstOrDefault();
            }
            if (ce != null)
            {
                gi.category = EhUtils.getCategory(ce.Text());
            }

            // Thumb
            var glthumb = e.GetElementByIdRecursive("glthumb");
            if (glthumb != null)
            {
                var img = glthumb.QuerySelectorAll("div:nth-Children(1)>img").FirstOrDefault();
                if (img != null)
                {
                    // Thumb size
                    var m = PATTERN_THUMB_SIZE.Match(img.GetAttribute("style"));
                    if (m.Success)
                    {
                        gi.thumbWidth = ParserUtils.parseInt(m.Groups[(2)].Value, 0);
                        gi.thumbHeight = ParserUtils.parseInt(m.Groups[(1)].Value, 0);
                    }
                    else
                    {
                        Log.w(TAG, "Can't parse gallery info thumb size");
                        gi.thumbWidth = 0;
                        gi.thumbHeight = 0;
                    }
                    // Thumb url
                    var url = img.GetAttribute("data-src");
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        url = img.GetAttribute("src");
                    }
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        url = null;
                    }
                    gi.thumb = EhUtils.handleThumbUrlResolution(settings,url);
                }

                // Pages
                var div = glthumb.QuerySelectorAll("div:nth-Children(2)>div:nth-Children(2)>div:nth-Children(2)").FirstOrDefault();
                if (div != null)
                {
                    var Match = PATTERN_PAGES.Match(div.Text());
                    if (Match.Success)
                    {
                        gi.pages = ParserUtils.parseInt(Match.Groups[(1)].Value, 0);
                    }
                }
            }

            IElement gl = default;
            // Try extended and thumbnail version
            if (gi.thumb == null)
            {
                gl = e.GetElementsByClassName("gl1e").FirstOrDefault();
                if (gl == null)
                {
                    gl = e.GetElementsByClassName("gl3t").FirstOrDefault();
                }
                if (gl != null)
                {
                    var img = gl.GetElementsByTagName("img").FirstOrDefault();
                    if (img != null)
                    {
                        // Thumb size
                        var m = PATTERN_THUMB_SIZE.Match(img.GetAttribute("style"));
                        if (m.Success)
                        {
                            gi.thumbWidth = ParserUtils.parseInt(m.Groups[(2)].Value, 0);
                            gi.thumbHeight = ParserUtils.parseInt(m.Groups[(1)].Value, 0);
                        }
                        else
                        {
                            Log.w(TAG, "Can't parse gallery info thumb size");
                            gi.thumbWidth = 0;
                            gi.thumbHeight = 0;
                        }
                        gi.thumb = EhUtils.handleThumbUrlResolution(settings,img.GetAttribute("src"));
                    }
                }
            }

            // Posted
            gi.favoriteSlot = -2;
            var postedId = "posted_" + gi.gid;
            var posted = e.GetElementByIdRecursive(postedId);
            if (posted != null)
            {
                gi.posted = posted.Text().Trim();
                gi.favoriteSlot = parseFavoriteSlot(posted.GetAttribute("style"));
            }
            else if (e.Children.ElementAtOrDefault(1)?.Children.ElementAtOrDefault(2)?.GetElementByIdRecursive(postedId) is IElement p)
            {
                //get posted from dms:Compact
                posted = p;
                gi.posted = posted.Text().Trim();
            }

            // Rating
            var ir = e.GetElementsByClassName("ir").FirstOrDefault();
            if (ir != null)
            {
                gi.rating = ParserUtils.parseFloat(parseRating(ir.GetAttribute("style")), -1.0f);
                // TODO The gallery may be rated even if it doesn't has one of these classes
                gi.rated = ir.ClassList.Contains("irr") || ir.ClassList.Contains("irg") || ir.ClassList.Contains("irb");
            }

            // Uploader and pages
            gl = e.GetElementsByClassName("glhide").FirstOrDefault();
            int uploaderIndex = 0;
            int pagesIndex = 1;
            if (gl == null)
            {
                // For extended
                gl = e.GetElementsByClassName("gl3e").FirstOrDefault();
                uploaderIndex = 3;
                pagesIndex = 4;
            }
            if (gl != null)
            {
                var children = gl.Children;
                if (children.Length > uploaderIndex)
                {
                    var a = children[(uploaderIndex)].Children.FirstOrDefault();
                    if (a != null)
                    {
                        gi.uploader = a.Text().Trim();
                    }
                }
                if (children.Length > pagesIndex)
                {
                    var Match = PATTERN_PAGES.Match(children[(pagesIndex)].Text());
                    if (Match.Success)
                    {
                        gi.pages = ParserUtils.parseInt(Match.Groups[(1)].Value, 0);
                    }
                }
            }
            // For thumbnail
            var gl5t = e.GetElementsByClassName("gl5t").FirstOrDefault();
            if (gl5t != null)
            {
                var div = gl5t.QuerySelectorAll("div:nth-Children(2)>div:nth-Children(2)").FirstOrDefault();
                if (div != null)
                {
                    var Match = PATTERN_PAGES.Match(div.Text());
                    if (Match.Success)
                    {
                        gi.pages = ParserUtils.parseInt(Match.Groups[(1)].Value, 0);
                    }
                }
            }

            gi.generateSLang();

            return gi;
        }

        public static Result parse(Settings settings,String body)
        {
            var result = new Result();
            var d = Utils.Document.parse(body);

            try
            {
                var ptt = d.getElementsByClass("ptt").FirstOrDefault();
                var es = ptt.Children[(0)].Children[(0)].Children;
                result.pages = int.Parse(es[(es.Length - 2)].Text().Trim());

                var e = es[(es.Length - 1)];
                if (e != null)
                {
                    e = e.Children.FirstOrDefault();
                    if (e != null)
                    {
                        String href = e.GetAttribute("href");
                        var Match = PATTERN_NEXT_PAGE.Match(href);
                        if (Match.Success)
                        {
                            result.nextPage = ParserUtils.parseInt(Match.Groups[(1)].Value, 0);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //ExceptionUtils.throwIfFatal(e);
                result.noWatchedTags = body.Contains("<p>You do not have any watched tags");
                if (body.Contains("No hits found</p>"))
                {
                    result.pages = 0;
                    //noinspection unchecked
                    result.galleryInfoList = new();
                    return result;
                }
                else if (d.getElementsByClass("ptt").Count() == 0)
                {
                    result.pages = 1;
                }
                else
                {
                    result.pages = int.MaxValue;
                }
            }

            try
            {
                var itg = d.getElementsByClass("itg").FirstOrDefault();
                IHtmlCollection<IElement> es;
                if ("table".Equals(itg.TagName, StringComparison.InvariantCultureIgnoreCase))
                {
                    es = itg.Children[(0)].Children;
                }
                else
                {
                    es = itg.Children;
                }
                List<GalleryInfo> list = new(es.Length);
                // First one is table header, skip it
                for (int i = 0; i < es.Length; i++)
                {
                    GalleryInfo gi = parseGalleryInfo(settings,es[(i)]);
                    if (null != gi)
                    {
                        list.Add(gi);
                    }
                }
                if (list.Count == 0)
                {
                    throw new ParseException("No gallery", body);
                }
                result.galleryInfoList = list;
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.printStackTrace();
                throw new ParseException("Can't parse gallery list", body);
            }

            return result;
        }

        public class Result
        {
            public int pages;
            public int nextPage;
            public bool noWatchedTags;
            public List<GalleryInfo> galleryInfoList;
        }
    }
}
