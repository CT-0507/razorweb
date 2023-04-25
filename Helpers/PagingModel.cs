using System;

namespace ex.Helpers
{
    public class PagingModel
    {
        public int currentPage { set; get; }

        public int countPages { set; get; }

        public Func<int?, string> generateUrl { get; set; }

    }
}