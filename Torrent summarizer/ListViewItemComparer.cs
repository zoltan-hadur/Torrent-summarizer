using ByteSizeLib;
using System;
using System.Collections;
using System.Windows.Forms;

namespace Torrent_summarizer
{
    class ListViewItemComparer : IComparer
    {
        private int column;
        private bool ascending;
        public int Column
        {
            get { return column; }
            set { if (value == 0 || value == 1) column = value; else column = 0; }
        }
        public bool Ascending
        {
            get { return ascending; }
            set { ascending = value; }
        }
        public ListViewItemComparer()
        {
            Column = 0;
            Ascending = true;
        }

        public int Compare(object x, object y)
        {
            switch (Column)
            {
                case 0: //Sort by name
                    if(Ascending)
                        return String.Compare(((ListViewItem)x).SubItems[Column].Text, ((ListViewItem)y).SubItems[Column].Text);
                    else
                        return String.Compare(((ListViewItem)y).SubItems[Column].Text, ((ListViewItem)x).SubItems[Column].Text);
                case 1: //Sort by size
                    double X = ByteSize.Parse(((ListViewItem)x).SubItems[Column].Text).Bytes;
                    double Y = ByteSize.Parse(((ListViewItem)y).SubItems[Column].Text).Bytes;
                    if (Ascending)
                        return (X < Y ? -1 : 1);
                    else
                        return (Y < X ? -1 : 1);
                default:
                    return 0;
            }
        }
    }
}
