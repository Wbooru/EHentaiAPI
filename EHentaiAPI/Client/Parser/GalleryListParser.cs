using AngleSharp.Dom;
using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.ExtendFunction;
using EHentaiAPI.Utils;
using EHentaiAPI.Utils.ExtensionMethods;
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

        private const string TAG = nameof(GalleryListParser);

        private static readonly Regex PATTERN_RATING = new Regex("\\d+px");
        private static readonly Regex PATTERN_THUMB_SIZE = new Regex("height:(\\d+)px;width:(\\d+)px");
        private static readonly Regex PATTERN_FAVORITE_SLOT = new Regex("background-color:rgba\\((\\d+),(\\d+),(\\d+),");
        private static readonly Regex PATTERN_PAGES = new Regex("(\\d+) page");
        private static readonly Regex PATTERN_NEXT_PAGE = new Regex("page=(\\d+)");

        private static readonly string[][] FAVORITE_SLOT_RGB = new string[][]{
            new string[]{"0", "0", "0"},
            new string[]{"240", "0", "0"},
            new string[] { "240", "160", "0" },
            new string[] { "208", "208", "0" },
            new string[] { "0", "128", "0" },
            new string[] { "144", "240", "64" },
            new string[] { "64", "176", "240" },
            new string[] { "0", "0", "240" },
            new string[] { "80", "0", "128" },
            new string[] { "224", "128", "224" },
    };

        private static int ParsePages(Utils.Document d, string body)
        {
            try
            {
                var es = d.GetElementsByClass("ptt").FirstOrDefault().Children[0].Children[0].Children;
                return int.Parse(es[es.Length - 2].Text().Trim());
            }
            catch (Exception)
            {
                //ExceptionUtils.throwIfFatal(e);
                throw new ParseException("Can't parse gallery list pages", body);
            }
        }

        private static string ParseRating(string ratingStyle)
        {
            var m = PATTERN_RATING.Match(ratingStyle);
            int num1 = int.MinValue;
            int num2 = int.MinValue;
            int rate = 5;
            string re;
            if (m.Success)
            {
                num1 = ParserUtils.ParseInt(m.Groups[0].Value.Replace("px", ""), int.MinValue);
            }
            if (m.Success)
            {
                num2 = ParserUtils.ParseInt(m.Groups[0].Value.Replace("px", ""), int.MinValue);
            }
            if (num1 == int.MinValue || num2 == int.MinValue)
            {
                return null;
            }
            rate -= num1 / 16;
            if (num2 == 21)
            {
                rate--;
                re = rate.ToString();
                re += ".5";
            }
            else
                re = rate.ToString();
            return re;
        }

        private static int ParseFavoriteSlot(string style)
        {
            var m = PATTERN_FAVORITE_SLOT.Match(style);
            if (m.Success)
            {
                string r = m.Groups[1].Value;
                string g = m.Groups[2].Value;
                string b = m.Groups[3].Value;
                int slot = 0;
                foreach (string[] rgb in FAVORITE_SLOT_RGB)
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

        private static GalleryInfo ParseGalleryInfo(Settings settings, IElement e)
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
                    if (parent != null && "a".Equals(parent.TagName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        a = parent;
                    }
                }
                if (a != null)
                {
                    GalleryDetailUrlParser.Result result = GalleryDetailUrlParser.Parse(a.GetAttributeEx("href"));
                    if (result != null)
                    {
                        gi.Gid = result.gid;
                        gi.Token = result.token;
                    }
                }

                var Children = glname;
                var children = glname.Children;
                while (children.Length != 0)
                {
                    Children = children[0];
                    children = Children.Children;
                }
                gi.Title = Children.Text().Trim();

                var tbody = glname.GetElementsByTagName("tbody").FirstOrDefault();
                if (tbody != null)
                {
                    List<string> tags = new();
                    GalleryTagGroup[] groups = GalleryDetailParser.ParseTagGroups(tbody.Children);
                    foreach (GalleryTagGroup Groups in groups)
                    {
                        for (int j = 0; j < Groups.Size; j++)
                        {
                            tags.Add(Groups.TagGroupName + ":" + Groups.GetTagAt(j));
                        }
                    }
                    gi.SimpleTags = tags.ToArray();
                }
            }
            if (gi.Title == null)
            {
                return null;
            }

            // Category
            gi.Category = EhUtils.UNKNOWN;
            var ce = e.GetElementByIdRecursive("cn");
            if (ce == null)
            {
                ce = e.GetElementsByClassName("cs").FirstOrDefault();
            }
            if (ce != null)
            {
                gi.Category = EhUtils.GetCategory(ce.Text());
            }

            // Thumb
            var glthumb = e.GetElementByIdRecursive("glthumb");
            if (glthumb != null)
            {
                var img = glthumb.QuerySelectorAll("div:nth-child(1)>img").FirstOrDefault();
                if (img != null)
                {
                    // Thumb size
                    var m = PATTERN_THUMB_SIZE.Match(img.GetAttributeEx("style"));
                    if (m.Success)
                    {
                        gi.ThumbWidth = ParserUtils.ParseInt(m.Groups[2].Value, 0);
                        gi.ThumbHeight = ParserUtils.ParseInt(m.Groups[1].Value, 0);
                    }
                    else
                    {
                        Log.W(TAG, "Can't parse gallery info thumb size");
                        gi.ThumbWidth = 0;
                        gi.ThumbHeight = 0;
                    }
                    // Thumb url
                    var url = img.GetAttributeEx("data-src");
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        url = img.GetAttributeEx("src");
                    }
                    if (string.IsNullOrWhiteSpace(url))
                    {
                        url = null;
                    }
                    gi.Thumb = EhUtils.HandleThumbUrlResolution(settings, url);
                }

                // Pages
                var div = glthumb.QuerySelectorAll("div:nth-child(2)>div:nth-child(2)>div:nth-child(2)").FirstOrDefault();
                if (div != null)
                {
                    var Match = PATTERN_PAGES.Match(div.Text());
                    if (Match.Success)
                    {
                        gi.Pages = ParserUtils.ParseInt(Match.Groups[1].Value, 0);
                    }
                }
            }

            IElement gl;
            // Try extended and thumbnail version
            if (gi.Thumb == null)
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
                        var m = PATTERN_THUMB_SIZE.Match(img.GetAttributeEx("style"));
                        if (m.Success)
                        {
                            gi.ThumbWidth = ParserUtils.ParseInt(m.Groups[2].Value, 0);
                            gi.ThumbHeight = ParserUtils.ParseInt(m.Groups[1].Value, 0);
                        }
                        else
                        {
                            Log.W(TAG, "Can't parse gallery info thumb size");
                            gi.ThumbWidth = 0;
                            gi.ThumbHeight = 0;
                        }
                        gi.Thumb = EhUtils.HandleThumbUrlResolution(settings, img.GetAttributeEx("src"));
                    }
                }
            }

            // Posted
            gi.favoriteSlot = -2;
            var postedId = "posted_" + gi.Gid;
            var posted = e.GetElementByIdRecursive(postedId);
            if (posted != null)
            {
                gi.Posted = posted.Text().Trim();
                gi.favoriteSlot = ParseFavoriteSlot(posted.GetAttributeEx("style"));
            }

            // Rating
            var ir = e.GetElementsByClassName("ir").FirstOrDefault();
            if (ir != null)
            {
                gi.Rating = ParserUtils.ParseFloat(ParseRating(ir.GetAttributeEx("style")), -1.0f);
                // TODO The gallery may be rated even if it doesn't has one of these classes
                gi.Rated = ir.ClassList.Contains("irr") || ir.ClassList.Contains("irg") || ir.ClassList.Contains("irb");
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
                    var a = children[uploaderIndex].Children.FirstOrDefault();
                    if (a != null)
                    {
                        gi.Uploader = a.Text().Trim();
                    }
                }
                if (children.Length > pagesIndex)
                {
                    var Match = PATTERN_PAGES.Match(children[pagesIndex].Text());
                    if (Match.Success)
                    {
                        gi.Pages = ParserUtils.ParseInt(Match.Groups[1].Value, 0);
                    }
                }
            }
            // For thumbnail
            var gl5t = e.GetElementsByClassName("gl5t").FirstOrDefault();
            if (gl5t != null)
            {
                var div = gl5t.QuerySelectorAll("div:nth-child(2)>div:nth-child(2)").FirstOrDefault();
                if (div != null)
                {
                    var Match = PATTERN_PAGES.Match(div.Text());
                    if (Match.Success)
                    {
                        gi.Pages = ParserUtils.ParseInt(Match.Groups[1].Value, 0);
                    }
                }
            }

            gi.GenerateSLang();

            return gi;
        }

        public static Result Parse(Settings settings, string body)
        {
            var result = new Result();
            var d = Utils.Document.Parse(body);

            try
            {
                var ptt = d.GetElementsByClass("ptt").FirstOrDefault();
                var es = ptt.Children[0].Children[0].Children;
                result.pages = int.Parse(es[es.Length - 2].Text().Trim());

                var e = es[es.Length - 1];
                if (e != null)
                {
                    e = e.Children.FirstOrDefault();
                    if (e != null)
                    {
                        string href = e.GetAttributeEx("href");
                        var Match = PATTERN_NEXT_PAGE.Match(href);
                        if (Match.Success)
                        {
                            result.nextPage = ParserUtils.ParseInt(Match.Groups[1].Value, 0);
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
                else if (d.GetElementsByClass("ptt").Length == 0)
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
                var itg = d.GetElementsByClass("itg").FirstOrDefault();
                IHtmlCollection<IElement> es;
                if ("table".Equals(itg.TagName, StringComparison.InvariantCultureIgnoreCase))
                {
                    es = itg.Children[0].Children;
                }
                else
                {
                    es = itg.Children;
                }
                List<GalleryInfo> list = new(es.Length);
                // First one is table header, skip it
                for (int i = 0; i < es.Length; i++)
                {
                    GalleryInfo gi = ParseGalleryInfo(settings, es[i]);
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
                e.PrintStackTrace();
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
