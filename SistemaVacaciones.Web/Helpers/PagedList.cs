using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace PagedList
{
    public interface IPagedList<T> : IList<T>
    {
        int PageCount { get; }
        int TotalItemCount { get; }
        int PageNumber { get; }
        int PageSize { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        bool IsFirstPage { get; }
        bool IsLastPage { get; }
    }

    public class StaticPagedList<T> : List<T>, IPagedList<T>
    {
        public int PageCount { get; }
        public int TotalItemCount { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < PageCount;
        public bool IsFirstPage => PageNumber == 1;
        public bool IsLastPage => PageNumber >= PageCount;

        public StaticPagedList(IEnumerable<T> subset, int pageNumber, int pageSize, int totalItemCount)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItemCount = totalItemCount;
            PageCount = totalItemCount > 0 ? (int)Math.Ceiling(totalItemCount / (double)pageSize) : 0;
            AddRange(subset);
        }
    }

    public static class PagedListExtensions
    {
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var list = source.ToList();
            int total = list.Count;
            var subset = list.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return new StaticPagedList<T>(subset, pageNumber, pageSize, total);
        }

        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            int total = source.Count();
            var subset = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new StaticPagedList<T>(subset, pageNumber, pageSize, total);
        }
    }

    public enum PagedListDisplayMode
    {
        Always,
        Never,
        IfNeeded
    }

    public class PagedListRenderOptions
    {
        public PagedListDisplayMode Display { get; set; } = PagedListDisplayMode.IfNeeded;
        public PagedListDisplayMode DisplayLinkToFirstPage { get; set; } = PagedListDisplayMode.IfNeeded;
        public PagedListDisplayMode DisplayLinkToLastPage { get; set; } = PagedListDisplayMode.IfNeeded;
        public PagedListDisplayMode DisplayLinkToPreviousPage { get; set; } = PagedListDisplayMode.IfNeeded;
        public PagedListDisplayMode DisplayLinkToNextPage { get; set; } = PagedListDisplayMode.IfNeeded;
        public bool DisplayEllipsesWhenNotShowingAllPageNumbers { get; set; } = true;
        public int MaximumPageNumbersToDisplay { get; set; } = 5;
        public string LinkToPreviousPageFormat { get; set; } = "«";
        public string LinkToNextPageFormat { get; set; } = "»";
        public string[] ContainerDivClasses { get; set; } = Array.Empty<string>();
        public string[] UlElementClasses { get; set; } = new[] { "pagination", "justify-content-center" };
        public string[] LiElementClasses { get; set; } = new[] { "page-item" };
    }
}

namespace PagedList.Mvc
{
    public static class HtmlHelperPagedListExtensions
    {
        public static IHtmlContent PagedListPager<T>(
            this IHtmlHelper htmlHelper,
            IPagedList<T> list,
            Func<int, string> generatePageUrl)
        {
            return PagedListPager(htmlHelper, list, generatePageUrl, new PagedListRenderOptions());
        }

        public static IHtmlContent PagedListPager<T>(
            this IHtmlHelper htmlHelper,
            IPagedList<T> list,
            Func<int, string> generatePageUrl,
            PagedListRenderOptions? options)
        {
            if (list == null || list.PageCount <= 1)
                return HtmlString.Empty;

            options ??= new PagedListRenderOptions();

            var ulClass = string.Join(" ", options.UlElementClasses.Length > 0 ? options.UlElementClasses : new[] { "pagination", "justify-content-center" });
            var liClass = string.Join(" ", options.LiElementClasses.Length > 0 ? options.LiElementClasses : new[] { "page-item" });

            var sb = new StringBuilder();
            sb.Append($"<nav aria-label=\"Paginacion\"><ul class=\"{ulClass}\">");

            // Previous
            if (list.HasPreviousPage)
            {
                sb.Append($"<li class=\"{liClass}\"><a class=\"page-link\" href=\"{generatePageUrl(list.PageNumber - 1)}\">{options.LinkToPreviousPageFormat}</a></li>");
            }
            else if (options.DisplayLinkToPreviousPage == PagedListDisplayMode.Always)
            {
                sb.Append($"<li class=\"{liClass} disabled\"><span class=\"page-link\">{options.LinkToPreviousPageFormat}</span></li>");
            }

            // Pages
            int startPage = Math.Max(1, list.PageNumber - (options.MaximumPageNumbersToDisplay / 2));
            int endPage = Math.Min(list.PageCount, startPage + options.MaximumPageNumbersToDisplay - 1);
            if (endPage - startPage + 1 < options.MaximumPageNumbersToDisplay)
            {
                startPage = Math.Max(1, endPage - options.MaximumPageNumbersToDisplay + 1);
            }

            for (int i = startPage; i <= endPage; i++)
            {
                if (i == list.PageNumber)
                {
                    sb.Append($"<li class=\"{liClass} active\" aria-current=\"page\"><span class=\"page-link\">{i}</span></li>");
                }
                else
                {
                    sb.Append($"<li class=\"{liClass}\"><a class=\"page-link\" href=\"{generatePageUrl(i)}\">{i}</a></li>");
                }
            }

            // Next
            if (list.HasNextPage)
            {
                sb.Append($"<li class=\"{liClass}\"><a class=\"page-link\" href=\"{generatePageUrl(list.PageNumber + 1)}\">{options.LinkToNextPageFormat}</a></li>");
            }
            else if (options.DisplayLinkToNextPage == PagedListDisplayMode.Always)
            {
                sb.Append($"<li class=\"{liClass} disabled\"><span class=\"page-link\">{options.LinkToNextPageFormat}</span></li>");
            }

            sb.Append("</ul></nav>");
            return new HtmlString(sb.ToString());
        }
    }
}
