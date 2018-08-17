using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PetShop.Web {

    public class CustomList : DataList {
        //Static constants
        protected const string HTML1 = "<table cellpadding=0 cellspacing=0><tr><td colspan=2>";
        protected const string HTML2 = "</td></tr><tr><td class=paging align=left>";
        protected const string HTML3 = "</td><td align=right class=paging>";
        protected const string HTML4 = "</td></tr></table>";
        private static readonly Regex RX = new Regex(@"^&page=\d+", RegexOptions.Compiled);
        private const string LINK_PREV = "<a href=?page={0}>&#060;&nbsp;Previous</a>";
        private const string LINK_MORE = "<a href=?page={0}>More&nbsp;&#062;</a>";
        private const string KEY_PAGE = "page";
        private const string COMMA = "?";
        private const string AMP = "&";

        protected string emptyText;
        private IList dataSource;
        private int pageSize = 10;
        private int currentPageIndex;
        private int itemCount;

        override public object DataSource {
            set {
                //This try catch block is to avoid issues with the VS.NET designer
                //The designer will try and bind a datasource which does not derive from ILIST
                try {
                    dataSource = (IList)value;
                    ItemCount = dataSource.Count;
                }
                catch {
                    dataSource = null;
                    ItemCount = 0;
                }
            }
        }

        public int PageSize {
            get { return pageSize; }
            set { pageSize = value; }
        }

        protected int PageCount {
            get { return (ItemCount - 1) / pageSize; }
        }

        virtual protected int ItemCount {
            get { return itemCount; }
            set { itemCount = value; }
        }

        virtual public int CurrentPageIndex {
            get { return currentPageIndex; }
            set { currentPageIndex = value; }
        }

        public string EmptyText {
            set { emptyText = value; }
        }

        public void SetPage(int index) {
            OnPageIndexChanged(new DataGridPageChangedEventArgs(null, index));
        }

        override protected void OnLoad(EventArgs e) {
            if (Visible) {
                string page = Context.Request[KEY_PAGE];
                int index = (page != null) ? int.Parse(page) : 0;
                SetPage(index);
            }
        }


        /// <summary>
        /// Overriden method to control how the page is rendered
        /// </summary>
        /// <param name="writer"></param>
        override protected void Render(HtmlTextWriter writer) {

            //Check there is some data attached
            if (ItemCount == 0) {
                writer.Write(emptyText);
                return;
            }

            //Mask the query
            string query = Context.Request.Url.Query.Replace(COMMA, AMP);
            query = RX.Replace(query, string.Empty);

           
            // Write out the first part of the control, the table header
            writer.Write(HTML1);

            // Call the inherited method
            base.Render(writer);
            
            // Write out a table row closure
            writer.Write(HTML2);

            //Determin whether next and previous buttons are required
            //Previous button?
            if (currentPageIndex > 0)
                writer.Write(string.Format(LINK_PREV, (currentPageIndex - 1) + query));

            //Close the table data tag
            writer.Write(HTML3);

            //Next button?
            if (currentPageIndex < PageCount)
                writer.Write(string.Format(LINK_MORE, (currentPageIndex + 1) + query));

            //Close the table
            writer.Write(HTML4);
        }

        override protected void OnDataBinding(EventArgs e) {

            //Work out which items we want to render to the page
            int start = CurrentPageIndex * pageSize;
            int size = Math.Min(pageSize, ItemCount - start);

            IList page = new ArrayList();

            //Add the relevant items from the datasource
            for (int i = 0; i < size; i++)
                page.Add(dataSource[start + i]);

            //set the base objects datasource
            base.DataSource = page;
            base.OnDataBinding(e);

        }

        public event DataGridPageChangedEventHandler PageIndexChanged;

        virtual protected void OnPageIndexChanged(DataGridPageChangedEventArgs e) {
            if (PageIndexChanged != null)
                PageIndexChanged(this, e);
        }
    }
}