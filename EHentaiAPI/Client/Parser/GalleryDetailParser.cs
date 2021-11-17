using AngleSharp.Dom;
using EHentaiAPI.Client.Data;
using EHentaiAPI.Client.Exceptions;
using EHentaiAPI.Utils;
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

        private readonly static GalleryTagGroup[] EMPTY_GALLERY_TAG_GROUP_ARRAY = new GalleryTagGroup[0];
        private readonly static GalleryCommentList EMPTY_GALLERY_COMMENT_ARRAY = new GalleryCommentList(new GalleryComment[0], false);

        private const string OFFENSIVE_STRING =
            "<p>(And if you choose to ignore this warning, you lose all rights to complain about it in the future.)</p>";
        private const string PINING_STRING =
                "<p>This gallery is pining for the fjords.</p>";

        public static GalleryDetail parse(EhUrl ehUrl, string body,string url)
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

            GalleryDetail galleryDetail = new GalleryDetail();
            galleryDetail.url = url;

            var document = Utils.Document.parse(body);
            parseDetail(ehUrl.getSettings(), galleryDetail, document, body);
            galleryDetail.tags = parseTagGroups(document);
            galleryDetail.comments = parseComments(document);
            galleryDetail.previewPages = parsePreviewPages(document, body);
            galleryDetail.previewSet = parsePreviewSet(ehUrl, document, body);
            return galleryDetail;
        }

        private static void parseDetail(Settings settings, GalleryDetail gd, Utils.Document d, string body)
        {
            var match = PATTERN_DETAIL.Match(body);
            if (match.Success)
            {
                gd.gid = ParserUtils.parseLong(match.Groups[1].Value, -1L);
                gd.token = match.Groups[3].Value;
                gd.apiUid = ParserUtils.parseLong(match.Groups[5].Value, -1L);
                gd.apiKey = match.Groups[7].Value;
            }
            else
            {
                throw new ParseException("Can't parse gallery detail", body);
            }
            if (gd.gid == -1L)
            {
                throw new ParseException("Can't parse gallery detail", body);
            }

            match = PATTERN_TORRENT.Match(body);
            if (match.Success)
            {
                gd.torrentUrl = ParserUtils.unescapeXml(ParserUtils.trim(match.Groups[1].Value));
                gd.torrentCount = ParserUtils.parseInt(match.Groups[2].Value, 0);
            }
            else
            {
                gd.torrentCount = 0;
                gd.torrentUrl = "";
            }

            match = PATTERN_ARCHIVE.Match(body);
            if (match.Success)
            {
                gd.archiveUrl = ParserUtils.unescapeXml(ParserUtils.trim(match.Groups[1].Value));
            }
            else
            {
                gd.archiveUrl = "";
            }

            try
            {
                var gm = d.getElementByClass("gm");

                // Thumb url
                var gd1 = gm.GetElementByIdRecursive("gd1");
                try
                {
                    gd.thumb = parseCoverStyle(settings, ParserUtils.trim(gd1.Children[0].GetAttributeEx("style")));
                }
                catch
                {
                    //ExceptionUtils.throwIfFatal(e);
                    gd.thumb = "";
                }

                // Title
                var gn = gm.GetElementByIdRecursive("gn");
                if (null != gn)
                {
                    gd.title = ParserUtils.trim(gn.Text());
                }
                else
                {
                    gd.title = "";
                }

                // Jpn title
                var gj = gm.GetElementByIdRecursive("gj");
                if (null != gj)
                {
                    gd.titleJpn = ParserUtils.trim(gj.Text());
                }
                else
                {
                    gd.titleJpn = "";
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
                    gd.category = EhUtils.getCategory(ce.Text());
                }
                catch (Exception)
                {
                    //ExceptionUtils.throwIfFatal(e);
                    gd.category = EhUtils.UNKNOWN;
                }

                // Uploader
                var gdn = gm.GetElementByIdRecursive("gdn");
                if (null != gdn)
                {
                    gd.uploader = ParserUtils.trim(gdn.Text());
                }
                else
                {
                    gd.uploader = "";
                }

                var gdd = gm.GetElementByIdRecursive("gdd");
                gd.posted = "";
                gd.parent = "";
                gd.visible = "";
                gd.visible = "";
                gd.size = "";
                gd.pages = 0;
                gd.favoriteCount = 0;
                try
                {
                    var es = gdd.Children[0].Children[0].Children;
                    for (int i = 0, n = es.Length; i < n; i++)
                    {
                        parseDetailInfo(gd, es[(i)], body);
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
                    gd.ratingCount = ParserUtils.parseInt(rating_count.Text(), 0);
                }
                else
                {
                    gd.ratingCount = 0;
                }

                // Rating
                var rating_label = gm.GetElementByIdRecursive("rating_label");
                if (null != rating_label)
                {
                    string ratingStr = ParserUtils.trim(rating_label.Text());
                    if ("Not Yet Rated".Equals(ratingStr))
                    {
                        gd.rating = -1.0f;
                    }
                    else
                    {
                        int index = ratingStr.IndexOf(' ');
                        if (index == -1 || index >= ratingStr.Length)
                        {
                            gd.rating = 0f;
                        }
                        else
                        {
                            gd.rating = ParserUtils.parseFloat(ratingStr.Substring(index + 1), 0f);
                        }
                    }
                }
                else
                {
                    gd.rating = -1.0f;
                }

                // isFavorited
                var gdf = gm.GetElementByIdRecursive("gdf");
                gd.isFavorited = null != gdf && !ParserUtils.trim(gdf.Text()).Equals("Add to Favorites");
                if (gdf != null)
                {
                    var favoriteName = ParserUtils.trim(gdf.Text());
                    if (favoriteName.Equals("Add to Favorites"))
                    {
                        gd.favoriteName = null;
                    }
                    else
                    {
                        gd.favoriteName = ParserUtils.trim(gdf.Text());
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
                var gnd = d.getElementById("gnd");
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
                        var element = elements[(i)];
                        var gi = new GalleryInfo();
                        var result = GalleryDetailUrlParser.parse(element.GetAttributeEx("href"));
                        if (result != null)
                        {
                            gi.gid = result.gid;
                            gi.token = result.token;
                            gi.title = ParserUtils.trim(element.Text());
                            gi.posted = dates[(i)];
                            gd.newerVersions.Add(gi);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                e.printStackTrace();
            }
        }

        // width:250px; height:356px; background:transparent url(https://exhentai.org/t/fe/1f/fe1fcfa9bf8fba2f03982eda0aa347cc9d6a6372-145921-1050-1492-jpg_250.jpg) 0 0 no-repeat
        private static string parseCoverStyle(Settings settings, string str)
        {
            var match = PATTERN_COVER.Match(str);
            if (match.Success)
            {
                return EhUtils.handleThumbUrlResolution(settings, match.Groups[3].Value);
            }
            else
            {
                return "";
            }
        }

        private static void parseDetailInfo(GalleryDetail gd, IElement e, string body)
        {
            var es = e.Children;
            if (es.Length < 2)
            {
                return;
            }

            string key = ParserUtils.trim(es[0].Text());
            string value = ParserUtils.trim(es[1].TextContent);
            if (key.StartsWith("Posted"))
            {
                gd.posted = value;
            }
            else if (key.StartsWith("Parent"))
            {
                var a = es[1].Children.FirstOrDefault();
                if (a != null)
                {
                    gd.parent = a.GetAttributeEx("href");
                }
            }
            else if (key.StartsWith("Visible"))
            {
                gd.visible = value;
            }
            else if (key.StartsWith("Language"))
            {
                gd.language = value;
            }
            else if (key.StartsWith("File Size"))
            {
                gd.size = value;
            }
            else if (key.StartsWith("Length"))
            {
                int index = value.IndexOf(' ');
                if (index >= 0)
                {
                    gd.pages = ParserUtils.parseInt(value.Substring(0, index - 0), 1);
                }
                else
                {
                    gd.pages = 1;
                }
            }
            else if (key.StartsWith("Favorited"))
            {
                switch (value)
                {
                    case "Never":
                        gd.favoriteCount = 0;
                        break;
                    case "Once":
                        gd.favoriteCount = 1;
                        break;
                    default:
                        int index = value.IndexOf(' ');
                        if (index == -1)
                        {
                            gd.favoriteCount = 0;
                        }
                        else
                        {
                            gd.favoriteCount = ParserUtils.parseInt(value.Substring(0, index - 0), 0);
                        }
                        break;
                }
            }
        }

        private static GalleryTagGroup parseTagGroup(IElement element)
        {
            try
            {
                var Group = new GalleryTagGroup();

                var nameSpace = element.Children[0].Text();
                // Remove last ':'
                nameSpace = nameSpace.Substring(0, nameSpace.Length - 1);
                Group.groupName = nameSpace;

                var tags = element.Children[1].Children;
                for (int i = 0, n = tags.Length; i < n; i++)
                {
                    string tag = tags[(i)].Text();
                    // Sometimes parody tag is followed with '|' and english translate, just remove them
                    int index = tag.IndexOf('|');
                    if (index >= 0)
                    {
                        tag = tag.Substring(0, index).Trim();
                    }
                    Group.addTag(tag);
                }

                return Group.size() > 0 ? Group : null;
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.printStackTrace();
                return null;
            }
        }

        /**
         * Parse tag groups with html parser
         */
        public static GalleryTagGroup[] parseTagGroups(Utils.Document document)
        {
            try
            {
                var taglist = document.getElementById("taglist");
                var tagGroups = taglist.Children[0].Children[0].Children;
                return parseTagGroups(tagGroups);
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.printStackTrace();
                return EMPTY_GALLERY_TAG_GROUP_ARRAY;
            }
        }

        public static GalleryTagGroup[] parseTagGroups(IEnumerable<IElement> trs)
        {
            try
            {
                List<GalleryTagGroup> list = new(trs.Count());
                for (int i = 0, n = trs.Count(); i < n; i++)
                {
                    GalleryTagGroup Group = parseTagGroup(trs.ElementAt(i));
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
                e.printStackTrace();
                return EMPTY_GALLERY_TAG_GROUP_ARRAY;
            }
        }

        /**
         * Parse tag groups with regular expressions
         */
        private static GalleryTagGroup[] parseTagGroups(string body)
        {
            List<GalleryTagGroup> list = new();

            Match m = PATTERN_TAG_GROUP.Match(body);
            while (m.Success)
            {
                GalleryTagGroup tagGroup = new GalleryTagGroup();
                tagGroup.groupName = ParserUtils.trim(m.Groups[1].Value);
                parseGroup(tagGroup, m.Groups[0].Value);
                list.Add(tagGroup);
            }

            return list.ToArray();
        }

        private static void parseGroup(GalleryTagGroup tagGroup, string body)
        {
            var m = PATTERN_TAG.Match(body);
            while (m.Success)
            {
                tagGroup.addTag(ParserUtils.trim(m.Groups[1].Value));
            }
        }

        public static GalleryComment parseComment(IElement element)
        {
            try
            {
                var comment = new GalleryComment();
                // Id
                var a = element.PreviousElementSibling;
                var name = a.GetAttributeEx("name");
                comment.id = int.Parse(ParserUtils.trim(name).Substring(1));
                // Editable, vote up and vote down
                var c4 = element.GetElementsByClassName("c4").FirstOrDefault();
                if (null != c4)
                {
                    if ("Uploader Comment".Equals(c4.Text()))
                    {
                        comment.uploader = true;
                    }
                    foreach (var e in c4.Children)
                    {
                        switch (e.Text())
                        {
                            case "Vote+":
                                comment.voteUpAble = true;
                                comment.voteUpEd = !(ParserUtils.trim(e.GetAttributeEx("style")).Length == 0);
                                break;
                            case "Vote-":
                                comment.voteDownAble = true;
                                comment.voteDownEd = !(ParserUtils.trim(e.GetAttributeEx("style")).Length == 0);
                                break;
                            case "Edit":
                                comment.editable = true;
                                break;
                        }
                    }
                }
                // Vote state
                var c7 = element.GetElementsByClassName("c7").FirstOrDefault();
                if (null != c7)
                {
                    comment.voteState = ParserUtils.trim(c7.Text());
                }
                // Score
                var c5 = element.GetElementsByClassName("c5").FirstOrDefault();
                if (null != c5)
                {
                    var es = c5.Children;
                    if (!(es.Length == 0))
                    {
                        comment.score = ParserUtils.parseInt(ParserUtils.trim(es[0].Text()), 0);
                    }
                }
                // time
                var c3 = element.GetElementsByClassName("c3").FirstOrDefault();
                var match = PATTERN_COMMENT_DATETIME.Match(c3.TextContent);
                var temp = match.Groups[1].Value;
                comment.time = DateTime.Parse(temp).ToUnixTimestamp();
                // user
                comment.user = c3.Children[0].Text();
                // comment
                comment.comment = element.GetElementsByClassName("c6").FirstOrDefault().Html();
                // last edited
                var c8 = element.GetElementsByClassName("c8").FirstOrDefault();
                if (c8 != null)
                {
                    var e = c8.Children.FirstOrDefault();
                    if (e != null)
                    {
                        comment.lastEdited = DateTime.Parse(temp).ToUnixTimestamp();
                    }
                }
                return comment;
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.printStackTrace();
                return null;
            }
        }

        /**
         * Parse comments with html parser
         */
        public static GalleryCommentList parseComments(Utils.Document document)
        {
            try
            {
                var cdiv = document.getElementById("cdiv");
                var c1s = cdiv.GetElementsByClassName("c1");

                List<GalleryComment> list = new(c1s.Length);
                for (int i = 0, n = c1s.Length; i < n; i++)
                {
                    GalleryComment comment = parseComment(c1s[(i)]);
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
                e.printStackTrace();
                return EMPTY_GALLERY_COMMENT_ARRAY;
            }
        }

        /**
         * Parse comments with regular expressions
         */
        public static GalleryComment[] parseComments(string body)
        {
            List<GalleryComment> list = new();

            Match m = PATTERN_COMMENT.Match(body);
            while (m.Success)
            {
                string webDateString = ParserUtils.trim(m.Groups[1].Value);
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
                comment.time = date.ToUnixTimestamp();
                comment.user = ParserUtils.trim(m.Groups[2].Value);
                comment.comment = m.Groups[3].Value;
                list.Add(comment);
            }

            return list.ToArray();
        }

        /**
         * Parse preview pages with html parser
         */
        public static int parsePreviewPages(EHentaiAPI.Utils.Document document, string body)
        {
            try
            {
                var elements = document.getElementsByClass("ptt").FirstOrDefault().Children[0].Children[0].Children;
                return int.Parse(elements[(elements.Length - 2)].Text());
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.printStackTrace();
                throw new ParseException("Can't parse preview pages", body);
            }
        }

        /**
         * Parse preview pages with regular expressions
         */
        public static int parsePreviewPages(string body)
        {
            Match m = PATTERN_PREVIEW_PAGES.Match(body);
            int previewPages = -1;
            if (m.Success)
            {
                previewPages = ParserUtils.parseInt(m.Groups[1].Value, -1);
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
        public static int parsePages(string body)
        {
            int pages = -1;

            Match m = PATTERN_PAGES.Match(body);
            if (m.Success)
            {
                pages = ParserUtils.parseInt(m.Groups[1].Value, -1);
            }

            if (pages < 0)
            {
                throw new ParseException("Parse pages error", body);
            }

            return pages;
        }

        public static PreviewSet parsePreviewSet(EhUrl ehUrl, EHentaiAPI.Utils.Document d, string body)
        {
            try
            {
                return parseLargePreviewSet(ehUrl, d, body);
            }
            catch (Exception)
            {
                return parseNormalPreviewSet(body);
            }
        }

        public static PreviewSet parsePreviewSet(EhUrl ehUrl, string body)
        {
            try
            {
                return parseLargePreviewSet(ehUrl, body);
            }
            catch (Exception)
            {
                return parseNormalPreviewSet(body);
            }
        }

        /**
         * Parse large previews with regular expressions
         */
        private static LargePreviewSet parseLargePreviewSet(EhUrl ehUrl, Utils.Document d, string body)
        {
            try
            {
                var largePreviewSet = new LargePreviewSet();
                var gdt = d.getElementById("gdt");
                var gdtls = gdt.GetElementsByClassName("gdtl");
                int n = gdtls.Length;
                if (n <= 0)
                {
                    throw new ParseException("Can't parse large preview", body);
                }
                for (int i = 0; i < n; i++)
                {
                    var element = gdtls[(i)].Children[0];
                    var pageUrl = element.GetAttributeEx("href");
                    element = element.Children[0];
                    var imageUrl = element.GetAttributeEx("src");
                    if (ehUrl.getSettings().getFixThumbUrl())
                    {
                        imageUrl = ehUrl.getFixedPreviewThumbUrl(imageUrl);
                    }
                    int index = int.Parse(element.GetAttributeEx("alt")) - 1;
                    largePreviewSet.addItem(index, imageUrl, pageUrl);
                }
                return largePreviewSet;
            }
            catch (Exception e)
            {
                //ExceptionUtils.throwIfFatal(e);
                e.printStackTrace();
                throw new ParseException("Can't parse large preview", body);
            }
        }

        /**
         * Parse large previews with regular expressions
         */
        private static LargePreviewSet parseLargePreviewSet(EhUrl ehUrl, string body)
        {
            Match m = PATTERN_LARGE_PREVIEW.Match(body);
            LargePreviewSet largePreviewSet = new LargePreviewSet();

            while (m.Success)
            {
                int index = ParserUtils.parseInt(m.Groups[2].Value, 0) - 1;
                if (index < 0)
                {
                    continue;
                }
                string imageUrl = ParserUtils.trim(m.Groups[3].Value);
                string pageUrl = ParserUtils.trim(m.Groups[1].Value);
                if (ehUrl.getSettings().getFixThumbUrl())
                {
                    imageUrl = ehUrl.getFixedPreviewThumbUrl(imageUrl);
                }
                largePreviewSet.addItem(index, imageUrl, pageUrl);
            }

            if (largePreviewSet.size() == 0)
            {
                throw new ParseException("Can't parse large preview", body);
            }

            return largePreviewSet;
        }

        /**
         * Parse normal previews with regular expressions
         */
        private static NormalPreviewSet parseNormalPreviewSet(string body)
        {
            Match m = PATTERN_NORMAL_PREVIEW.Match(body);
            NormalPreviewSet normalPreviewSet = new NormalPreviewSet();
            while (m.Success)
            {
                int position = ParserUtils.parseInt(m.Groups[6].Value, 0) - 1;
                if (position < 0)
                {
                    continue;
                }
                string imageUrl = ParserUtils.trim(m.Groups[3].Value);
                int xOffset = ParserUtils.parseInt(m.Groups[4].Value, 0);
                int yOffset = 0;
                int width = ParserUtils.parseInt(m.Groups[1].Value, 0);
                if (width <= 0)
                {
                    continue;
                }
                int height = ParserUtils.parseInt(m.Groups[2].Value, 0);
                if (height <= 0)
                {
                    continue;
                }
                string pageUrl = ParserUtils.trim(m.Groups[5].Value);
                normalPreviewSet.addItem(position, imageUrl, xOffset, yOffset, width, height, pageUrl);

                m = m.NextMatch();
            }

            if (normalPreviewSet.size() == 0)
            {
                throw new ParseException("Can't parse normal preview", body);
            }

            return normalPreviewSet;
        }
    }
}
