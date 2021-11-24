using AngleSharp.Dom;
using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Exceptions;
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
    class GalleryDetailParser
    {
        private readonly static Regex PATTERN_ERROR = new Regex("<div class=\"d\">\n<p>([^<]+)</p>");
        private readonly static Regex PATTERN_COMMENT_DATETIME = new Regex(@"Posted\s*on\s*(.+?)\s*by");
        private readonly static Regex PATTERN_DETAIL = new Regex(@"var gid = (\d+);\s*?(\n|\r|\r\n)?\s*?var token = \""([a-f0-9]+)\"";\s*?(\n|\r|\r\n)?\s*?var apiuid = ([\-\d]+);\s*?(\n|\r|\r\n)?\s*?var apikey = \""([a-f0-9]+)\"";");
        private readonly static Regex PATTERN_TORRENT = new Regex("<a[^<>]*onclick=\"return popUp\\('([^']+)'[^)]+\\)\">Torrent Download[^<]+(\\d+)[^<]+</a");
        private readonly static Regex PATTERN_ARCHIVE = new Regex("<a[^<>]*onclick=\"return popUp\\('([^']+)'[^)]+\\)\">Archive Download</a>");
        private readonly static Regex PATTERN_COVER = new Regex("width:(\\d+)px; height:(\\d+)px.+?url\\((.+?)\\)");
        private readonly static Regex PATTERN_TAG_GROUP = new Regex("<tr><td[^<>]+>([\\w\\s]+):</td><td>(?:<div[^<>]+><a[^<>]+>[\\w\\s]+</a></div>)+</td></tr>");
        private readonly static Regex PATTERN_TAG = new Regex("<div[^<>]+><a[^<>]+>([\\w\\s]+)</a></div>");
        private readonly static Regex PATTERN_COMMENT = new Regex("<div class=\"c3\">Posted on ([^<>]+) by: &nbsp; <a[^<>]+>([^<>]+)</a>.+?<div class=\"c6\"[^>]*>(.+?)</div><div class=\"c[78]\"");
        private readonly static Regex PATTERN_PAGES = new Regex("<tr><td[^<>]*>Length:</td><td[^<>]*>([\\d,]+) pages</td></tr>");
        private readonly static Regex PATTERN_PREVIEW_PAGES = new Regex("<td[^>]+><a[^>]+>([\\d,]+)</a></td><td[^>]+>(?:<a[^>]+>)?&gt;(?:</a>)?</td>");
        private readonly static Regex PATTERN_NORMAL_PREVIEW = new Regex("<div class=\"gdtm\"[^<>]*><div[^<>]*width:(\\d+)[^<>]*height:(\\d+)[^<>]*\\((.+?)\\)[^<>]*-(\\d+)px[^<>]*><a[^<>]*href=\"(.+?)\"[^<>]*><img alt=\"([\\d,]+)\"");
        private readonly static Regex PATTERN_LARGE_PREVIEW = new Regex("<div class=\"gdtl\".+?<a href=\"(.+?)\"><img alt=\"([\\d,]+)\".+?src=\"(.+?)\"");
        private readonly static Regex PATTERN_NEWER_DATE = new Regex(", added (.+?)<br />");

        private readonly static GalleryTagGroup[] EMPTY_GALLERY_TAG_GROUP_ARRAY = Array.Empty<GalleryTagGroup>();
        private readonly static GalleryCommentList EMPTY_GALLERY_COMMENT_ARRAY = new GalleryCommentList(Array.Empty<GalleryComment>(), false);

        private const string OFFENSIVE_STRING =
            "<p>(And if you choose to ignore this warning, you lose all rights to complain about it in the future.)</p>";
        private const string PINING_STRING =
                "<p>This gallery is pining for the fjords.</p>";

        public static GalleryDetail Parse(EhUrl ehUrl, string body, string url)
        {
            if (body.Contains(OFFENSIVE_STRING))
            {
                throw new OffensiveException();
            }

            if (body.Contains(PINING_STRING))
            {
                throw new PiningException();
            }

            // Error info
            var m = PATTERN_ERROR.Match(body);
            if (m.Success)
            {
                throw new EhException(m.Groups[1].Value);
            }

            var galleryDetail = new GalleryDetail();
            galleryDetail.Url = url;

            var document = Utils.Document.Parse(body);
            ParseDetail(ehUrl.GetSettings(), galleryDetail, document, body);
            galleryDetail.Tags = ParseTagGroups(document);
            galleryDetail.Comments = ParseComments(document);
            galleryDetail.PreviewPages = ParsePreviewPages(document, body);
            galleryDetail.PreviewSet = ParsePreviewSet(ehUrl, document, body);
            return galleryDetail;
        }

        private static void ParseDetail(Settings settings, GalleryDetail gd, Utils.Document d, string body)
        {
            var match = PATTERN_DETAIL.Match(body);
            if (match.Success)
            {
                gd.Gid = ParserUtils.ParseLong(match.Groups[1].Value, -1L);
                gd.Token = match.Groups[3].Value;
                gd.ApiUid = ParserUtils.ParseLong(match.Groups[5].Value, -1L);
                gd.ApiKey = match.Groups[7].Value;
            }
            else
            {
                throw new ParseException("Can't parse gallery detail", body);
            }
            if (gd.Gid == -1L)
            {
                throw new ParseException("Can't parse gallery detail", body);
            }

            match = PATTERN_TORRENT.Match(body);
            if (match.Success)
            {
                gd.TorrentUrl = ParserUtils.UnescapeXml(ParserUtils.Trim(match.Groups[1].Value));
                gd.TorrentCount = ParserUtils.ParseInt(match.Groups[2].Value, 0);
            }
            else
            {
                gd.TorrentCount = 0;
                gd.TorrentUrl = "";
            }

            match = PATTERN_ARCHIVE.Match(body);
            if (match.Success)
            {
                gd.ArchiveUrl = ParserUtils.UnescapeXml(ParserUtils.Trim(match.Groups[1].Value));
            }
            else
            {
                gd.ArchiveUrl = "";
            }

            try
            {
                var gm = d.GetElementByClass("gm");

                // Thumb url
                var gd1 = gm.GetElementByIdRecursive("gd1");
                try
                {
                    gd.Thumb = ParseCoverStyle(settings, ParserUtils.Trim(gd1.Children[0].GetAttributeEx("style")));
                }
                catch
                {
                    //ExceptionUtils.throwIfFatal(e);
                    gd.Thumb = "";
                }

                // Title
                var gn = gm.GetElementByIdRecursive("gn");
                if (null != gn)
                {
                    gd.Title = ParserUtils.Trim(gn.Text());
                }
                else
                {
                    gd.Title = "";
                }

                // Jpn title
                var gj = gm.GetElementByIdRecursive("gj");
                if (null != gj)
                {
                    gd.TitleJpn = ParserUtils.Trim(gj.Text());
                }
                else
                {
                    gd.TitleJpn = "";
                }

                // Category
                var gdc = gm.GetElementByIdRecursive("gdc");
                try
                {
                    var ce = gdc.GetElementsByClassName("cn").FirstOrDefault();
                    if (ce == null)
                    {
                        ce = gdc.GetElementsByClassName("cs").FirstOrDefault();
                    }
                    gd.Category = EhUtils.GetCategory(ce.Text());
                }
                catch (Exception)
                {
                    //ExceptionUtils.throwIfFatal(e);
                    gd.Category = EhUtils.UNKNOWN;
                }

                // Uploader
                var gdn = gm.GetElementByIdRecursive("gdn");
                if (null != gdn)
                {
                    gd.Uploader = ParserUtils.Trim(gdn.Text());
                }
                else
                {
                    gd.Uploader = "";
                }

                var gdd = gm.GetElementByIdRecursive("gdd");
                gd.Posted = "";
                gd.Parent = "";
                gd.Visible = "";
                gd.Visible = "";
                gd.Size = "";
                gd.Pages = 0;
                gd.FavoriteCount = 0;
                try
                {
                    var es = gdd.Children[0].Children[0].Children;
                    for (int i = 0, n = es.Length; i < n; i++)
                    {
                        ParseDetailInfo(gd, es[i]);
                    }
                }
                catch
                {
                    //ExceptionUtils.throwIfFatal(e);
                    // Ignore
                }

                // Rating count
                var rating_count = gm.GetElementByIdRecursive("rating_count");
                if (null != rating_count)
                {
                    gd.RatingCount = ParserUtils.ParseInt(rating_count.Text(), 0);
                }
                else
                {
                    gd.RatingCount = 0;
                }

                // Rating
                var rating_label = gm.GetElementByIdRecursive("rating_label");
                if (null != rating_label)
                {
                    string ratingStr = ParserUtils.Trim(rating_label.Text());
                    if ("Not Yet Rated".Equals(ratingStr))
                    {
                        gd.Rating = -1.0f;
                    }
                    else
                    {
                        int index = ratingStr.IndexOf(' ');
                        if (index == -1 || index >= ratingStr.Length)
                        {
                            gd.Rating = 0f;
                        }
                        else
                        {
                            gd.Rating = ParserUtils.ParseFloat(ratingStr.Substring(index + 1), 0f);
                        }
                    }
                }
                else
                {
                    gd.Rating = -1.0f;
                }

                // isFavorited
                var gdf = gm.GetElementByIdRecursive("gdf");
                gd.IsFavorited = null != gdf && !ParserUtils.Trim(gdf.Text()).Equals("Add to Favorites");
                if (gdf != null)
                {
                    var favoriteName = ParserUtils.Trim(gdf.Text());
                    if (favoriteName.Equals("Add to Favorites"))
                    {
                        gd.favoriteName = null;
                    }
                    else
                    {
                        gd.favoriteName = ParserUtils.Trim(gdf.Text());
                    }
                }
            }
            catch (Exception)
            {
                //ExceptionUtils.throwIfFatal(e);
                throw new ParseException("Can't parse gallery detail", body);
            }

            // newer version
            try
            {
                var gnd = d.GetElementById("gnd");
                if (gnd != null)
                {
                    match = PATTERN_NEWER_DATE.Match(body);
                    var dates = new List<string>();
                    while (match.Success)
                    {
                        dates.Add(match.Groups[1].Value);
                    }
                    var elements = gnd.QuerySelectorAll("a");
                    for (int i = 0; i < elements.Length; i++)
                    {
                        var element = elements[i];
                        var gi = new GalleryInfo();
                        var result = GalleryDetailUrlParser.Parse(element.GetAttributeEx("href"));
                        if (result != null)
                        {
                            gi.Gid = result.gid;
                            gi.Token = result.token;
                            gi.Title = ParserUtils.Trim(element.Text());
                            gi.Posted = dates[i];
                            gd.NewerVersions.Add(gi);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
        }

        // width:250px; height:356px; background:transparent url(https://exhentai.org/t/fe/1f/fe1fcfa9bf8fba2f03982eda0aa347cc9d6a6372-145921-1050-1492-jpg_250.jpg) 0 0 no-repeat
        private static string ParseCoverStyle(Settings settings, string str)
        {
            var match = PATTERN_COVER.Match(str);
            if (match.Success)
            {
                return EhUtils.HandleThumbUrlResolution(settings, match.Groups[3].Value);
            }
            else
            {
                return "";
            }
        }

        private static void ParseDetailInfo(GalleryDetail gd, IElement e)
        {
            var es = e.Children;
            if (es.Length < 2)
            {
                return;
            }

            string key = ParserUtils.Trim(es[0].Text());
            string value = ParserUtils.Trim(es[1].TextContent);
            if (key.StartsWith("Posted"))
            {
                gd.Posted = value;
            }
            else if (key.StartsWith("Parent"))
            {
                var a = es[1].Children.FirstOrDefault();
                if (a != null)
                {
                    gd.Parent = a.GetAttributeEx("href");
                }
            }
            else if (key.StartsWith("Visible"))
            {
                gd.Visible = value;
            }
            else if (key.StartsWith("Language"))
            {
                gd.Language = value;
            }
            else if (key.StartsWith("File Size"))
            {
                gd.Size = value;
            }
            else if (key.StartsWith("Length"))
            {
                int index = value.IndexOf(' ');
                if (index >= 0)
                {
                    gd.Pages = ParserUtils.ParseInt(value.Substring(0, index - 0), 1);
                }
                else
                {
                    gd.Pages = 1;
                }
            }
            else if (key.StartsWith("Favorited"))
            {
                switch (value)
                {
                    case "Never":
                        gd.FavoriteCount = 0;
                        break;
                    case "Once":
                        gd.FavoriteCount = 1;
                        break;
                    default:
                        int index = value.IndexOf(' ');
                        if (index == -1)
                        {
                            gd.FavoriteCount = 0;
                        }
                        else
                        {
                            gd.FavoriteCount = ParserUtils.ParseInt(value.Substring(0, index - 0), 0);
                        }
                        break;
                }
            }
        }

        private static GalleryTagGroup ParseTagGroup(IElement element)
        {
            try
            {
                var Group = new GalleryTagGroup();

                var nameSpace = element.Children[0].Text();
                // Remove last ':'
                nameSpace = nameSpace.Substring(0, nameSpace.Length - 1);
                Group.TagGroupName = nameSpace;

                var tags = element.Children[1].Children;
                for (int i = 0, n = tags.Length; i < n; i++)
                {
                    string tag = tags[i].Text();
                    // Sometimes parody tag is followed with '|' and english translate, just remove them
                    int index = tag.IndexOf('|');
                    if (index >= 0)
                    {
                        tag = tag.Substring(0, index).Trim();
                    }
                    Group.AddTag(tag);
                }

                return Group.Size > 0 ? Group : null;
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.PrintStackTrace();
                return null;
            }
        }

        /**
         * Parse tag groups with html parser
         */
        public static GalleryTagGroup[] ParseTagGroups(Utils.Document document)
        {
            try
            {
                var taglist = document.GetElementById("taglist");
                var tagGroups = taglist.Children[0].Children[0].Children;
                return ParseTagGroups(tagGroups);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.PrintStackTrace();
                return EMPTY_GALLERY_TAG_GROUP_ARRAY;
            }
        }

        public static GalleryTagGroup[] ParseTagGroups(IEnumerable<IElement> trs)
        {
            try
            {
                List<GalleryTagGroup> list = new(trs.Count());
                for (int i = 0, n = trs.Count(); i < n; i++)
                {
                    GalleryTagGroup Group = ParseTagGroup(trs.ElementAt(i));
                    if (null != Group)
                    {
                        list.Add(Group);
                    }
                }
                return list.ToArray();
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.PrintStackTrace();
                return EMPTY_GALLERY_TAG_GROUP_ARRAY;
            }
        }

        /**
         * Parse tag groups with regular expressions
         */
        private static GalleryTagGroup[] ParseTagGroups(string body)
        {
            List<GalleryTagGroup> list = new();

            Match m = PATTERN_TAG_GROUP.Match(body);
            while (m.Success)
            {
                GalleryTagGroup tagGroup = new GalleryTagGroup();
                tagGroup.TagGroupName = ParserUtils.Trim(m.Groups[1].Value);
                ParseGroup(tagGroup, m.Groups[0].Value);
                list.Add(tagGroup);

                m = m.NextMatch();
            }

            return list.ToArray();
        }

        private static void ParseGroup(GalleryTagGroup tagGroup, string body)
        {
            var m = PATTERN_TAG.Match(body);
            while (m.Success)
            {
                tagGroup.AddTag(ParserUtils.Trim(m.Groups[1].Value));
                m = m.NextMatch();
            }
        }

        public static GalleryComment ParseComment(IElement element)
        {
            try
            {
                var comment = new GalleryComment();
                // Id
                var a = element.PreviousElementSibling;
                var name = a.GetAttributeEx("name");
                comment.Id = int.Parse(ParserUtils.Trim(name).Substring(1));
                // Editable, vote up and vote down
                var c4 = element.GetElementsByClassName("c4").FirstOrDefault();
                if (null != c4)
                {
                    if ("Uploader Comment".Equals(c4.Text()))
                    {
                        comment.Uploader = true;
                    }
                    foreach (var e in c4.Children)
                    {
                        switch (e.Text())
                        {
                            case "Vote+":
                                comment.VoteUpAble = true;
                                comment.VoteUpEd = !(ParserUtils.Trim(e.GetAttributeEx("style")).Length == 0);
                                break;
                            case "Vote-":
                                comment.VoteDownAble = true;
                                comment.VoteDownEd = !(ParserUtils.Trim(e.GetAttributeEx("style")).Length == 0);
                                break;
                            case "Edit":
                                comment.Editable = true;
                                break;
                        }
                    }
                }
                // Vote state
                var c7 = element.GetElementsByClassName("c7").FirstOrDefault();
                if (null != c7)
                {
                    comment.VoteState = ParserUtils.Trim(c7.Text());
                }
                // Score
                var c5 = element.GetElementsByClassName("c5").FirstOrDefault();
                if (null != c5)
                {
                    var es = c5.Children;
                    if (!(es.Length == 0))
                    {
                        comment.Score = ParserUtils.ParseInt(ParserUtils.Trim(es[0].Text()), 0);
                    }
                }
                // time
                var c3 = element.GetElementsByClassName("c3").FirstOrDefault();
                var match = PATTERN_COMMENT_DATETIME.Match(c3.TextContent);
                var temp = match.Groups[1].Value;
                comment.Time = DateTime.Parse(temp).ToUnixTimestamp();
                // user
                comment.User = c3.Children[0].Text();
                // comment
                comment.Comment = element.GetElementsByClassName("c6").FirstOrDefault().Html();
                // last edited
                var c8 = element.GetElementsByClassName("c8").FirstOrDefault();
                if (c8 != null)
                {
                    var e = c8.Children.FirstOrDefault();
                    if (e != null)
                    {
                        comment.LastEdited = DateTime.Parse(temp).ToUnixTimestamp();
                    }
                }
                return comment;
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.PrintStackTrace();
                return null;
            }
        }

        /**
         * Parse comments with html parser
         */
        public static GalleryCommentList ParseComments(Utils.Document document)
        {
            try
            {
                var cdiv = document.GetElementById("cdiv");
                var c1s = cdiv.GetElementsByClassName("c1");

                List<GalleryComment> list = new(c1s.Length);
                for (int i = 0, n = c1s.Length; i < n; i++)
                {
                    GalleryComment comment = ParseComment(c1s[i]);
                    if (null != comment)
                    {
                        list.Add(comment);
                    }
                }

                var chd = cdiv.GetElementByIdRecursive("chd");
                var hasMore = false;

                Queue<IElement> queue = new Queue<IElement>();
                queue.Enqueue(chd);
                while (queue.Count != 0)
                {
                    var node = queue.Dequeue();
                    if (node is Element e && e.Text().Equals("click to show all"))
                    {
                        hasMore = true;
                        break;
                    }
                    foreach (var child in node.Children)
                        queue.Enqueue(child);
                }


                return new GalleryCommentList(list.ToArray(), hasMore);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.PrintStackTrace();
                return EMPTY_GALLERY_COMMENT_ARRAY;
            }
        }

        /**
         * Parse comments with regular expressions
         */
        public static GalleryComment[] ParseComments(string body)
        {
            List<GalleryComment> list = new();

            Match m = PATTERN_COMMENT.Match(body);
            while (m.Success)
            {
                string webDateString = ParserUtils.Trim(m.Groups[1].Value);
                DateTime date;
                try
                {
                    date = DateTime.Parse(webDateString);
                }
                catch (ParseException)
                {
                    date = new DateTime(0L);
                }
                GalleryComment comment = new GalleryComment();
                comment.Time = date.ToUnixTimestamp();
                comment.User = ParserUtils.Trim(m.Groups[2].Value);
                comment.Comment = m.Groups[3].Value;
                list.Add(comment);

                m = m.NextMatch();
            }

            return list.ToArray();
        }

        /**
         * Parse preview pages with html parser
         */
        public static int ParsePreviewPages(EHentaiAPI.Utils.Document document, string body)
        {
            try
            {
                var elements = document.GetElementsByClass("ptt").FirstOrDefault().Children[0].Children[0].Children;
                return int.Parse(elements[elements.Length - 2].Text());
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.PrintStackTrace();
                throw new ParseException("Can't parse preview pages", body);
            }
        }

        /**
         * Parse preview pages with regular expressions
         */
        public static int ParsePreviewPages(string body)
        {
            Match m = PATTERN_PREVIEW_PAGES.Match(body);
            int previewPages = -1;
            if (m.Success)
            {
                previewPages = ParserUtils.ParseInt(m.Groups[1].Value, -1);
            }

            if (previewPages <= 0)
            {
                throw new ParseException("Parse preview page count error", body);
            }

            return previewPages;
        }

        /**
         * Parse pages with regular expressions
         */
        public static int ParsePages(string body)
        {
            int pages = -1;

            Match m = PATTERN_PAGES.Match(body);
            if (m.Success)
            {
                pages = ParserUtils.ParseInt(m.Groups[1].Value, -1);
            }

            if (pages < 0)
            {
                throw new ParseException("Parse pages error", body);
            }

            return pages;
        }

        public static PreviewSet ParsePreviewSet(EhUrl ehUrl, EHentaiAPI.Utils.Document d, string body)
        {
            try
            {
                return ParseLargePreviewSet(ehUrl, d, body);
            }
            catch (Exception)
            {
                return ParseNormalPreviewSet(body);
            }
        }

        public static PreviewSet ParsePreviewSet(EhUrl ehUrl, string body)
        {
            try
            {
                return ParseLargePreviewSet(ehUrl, body);
            }
            catch (Exception)
            {
                return ParseNormalPreviewSet(body);
            }
        }

        /**
         * Parse large previews with regular expressions
         */
        private static LargePreviewSet ParseLargePreviewSet(EhUrl ehUrl, Utils.Document d, string body)
        {
            try
            {
                var largePreviewSet = new LargePreviewSet();
                var gdt = d.GetElementById("gdt");
                var gdtls = gdt.GetElementsByClassName("gdtl");
                int n = gdtls.Length;
                if (n <= 0)
                {
                    throw new ParseException("Can't parse large preview (meybe need user sign in.)", body);
                }
                for (int i = 0; i < n; i++)
                {
                    var element = gdtls[i].Children[0];
                    var pageUrl = element.GetAttributeEx("href");
                    element = element.Children[0];
                    var imageUrl = element.GetAttributeEx("src");
                    if (ehUrl.GetSettings().GetFixThumbUrl())
                    {
                        imageUrl = ehUrl.GetFixedPreviewThumbUrl(imageUrl);
                    }
                    int index = int.Parse(element.GetAttributeEx("alt")) - 1;
                    largePreviewSet.AddItem(index, imageUrl, pageUrl);
                }
                return largePreviewSet;
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.PrintStackTrace();
                throw new ParseException("Can't parse large preview (meybe need user sign in.)", body);
            }
        }

        /**
         * Parse large previews with regular expressions
         */
        private static LargePreviewSet ParseLargePreviewSet(EhUrl ehUrl, string body)
        {
            Match m = PATTERN_LARGE_PREVIEW.Match(body);
            LargePreviewSet largePreviewSet = new LargePreviewSet();

            while (m.Success)
            {
                int index = ParserUtils.ParseInt(m.Groups[2].Value, 0) - 1;
                if (index < 0)
                {
                    continue;
                }
                string imageUrl = ParserUtils.Trim(m.Groups[3].Value);
                string pageUrl = ParserUtils.Trim(m.Groups[1].Value);
                if (ehUrl.GetSettings().GetFixThumbUrl())
                {
                    imageUrl = ehUrl.GetFixedPreviewThumbUrl(imageUrl);
                }
                largePreviewSet.AddItem(index, imageUrl, pageUrl);
                m = m.NextMatch();
            }

            if (largePreviewSet.Size == 0)
            {
                throw new ParseException("Can't parse large preview", body);
            }

            return largePreviewSet;
        }

        /**
         * Parse normal previews with regular expressions
         */
        private static NormalPreviewSet ParseNormalPreviewSet(string body)
        {
            Match m = PATTERN_NORMAL_PREVIEW.Match(body);
            NormalPreviewSet normalPreviewSet = new NormalPreviewSet();
            while (m.Success)
            {
                int position = ParserUtils.ParseInt(m.Groups[6].Value, 0) - 1;
                if (position < 0)
                    continue;
                string imageUrl = ParserUtils.Trim(m.Groups[3].Value);
               
                int width = ParserUtils.ParseInt(m.Groups[1].Value, 0);
                if (width <= 0)
                    continue;
                int height = ParserUtils.ParseInt(m.Groups[2].Value, 0);
                if (height <= 0)
                    continue;
                string pageUrl = ParserUtils.Trim(m.Groups[5].Value);
                normalPreviewSet.AddItem(position, imageUrl, pageUrl);

                m = m.NextMatch();
            }

            if (normalPreviewSet.Size == 0)
            {
                throw new ParseException("Can't parse normal preview", body);
            }

            return normalPreviewSet;
        }
    }
}
