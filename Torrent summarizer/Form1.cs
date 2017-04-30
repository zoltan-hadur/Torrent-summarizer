using BencodeNET.Parsing;
using BencodeNET.Torrents;
using ByteSizeLib;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Torrent_summarizer
{
    public partial class Form1 : Form
    {
        long size = 0;
        ListViewItemComparer sorter = new ListViewItemComparer();
        public Form1()
        {
            InitializeComponent();
            lstTorrents.ListViewItemSorter = sorter;
        }

        private void lstTorrents_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == sorter.Column)
            {//If we already sorting that column, just flip the order of sorting: ascending->descending or descending->ascending
                sorter.Ascending = !sorter.Ascending;
            }
            else
            {//Else change what column we will sort, with a default ascending order
                sorter.Column = e.Column;
                sorter.Ascending = true;
            }
            lstTorrents.Sort();
        }

        private void lstTorrents_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))    //Torrent drag and drops are file drops
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                bool allow = true;
                foreach (var file in files)                     //Check if all file in the list is a torrent file
                {
                    if (Path.GetExtension(file) != ".torrent")
                    {
                        allow = false;
                    }
                }
                if (allow)                                      //If every file is a torrent file
                {
                    if (lstTorrents.Items.Count == 0)           //Check if it is the first time running this function, or the program has been cleared
                    {
                        var icon = Icon.ExtractAssociatedIcon(files[0]);    //Get the associated icon to the torrent file
                        imageList.Images.Add("torrent", icon);              //The listview will use that icon
                        lstTorrents.SmallImageList = imageList;
                    }
                    e.Effect = DragDropEffects.Copy;            //Thus we will be enable to drop the files into the list view
                }
                else
                {
                    e.Effect = DragDropEffects.None;            //If atleast 1 file is not a torrent file
                }
            }
        }

        private void lstTorrents_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var parser = new BencodeParser();   //To extract torrent informations
            foreach (var file in files)
            {
                var torrent = parser.Parse<Torrent>(file);
                var file_name = file.Substring(file.LastIndexOf('\\') + 1); //Extract torrent file name from it's full path
                var file_size = ByteSize.FromBytes(torrent.TotalSize);
                lstTorrents.Items.Add(new ListViewItem(new[] { file_name, file_size.ToString("0.000 GB") }, "torrent"));
                size = size + torrent.TotalSize;
            }
            lblFileCount.Text = lstTorrents.Items.Count.ToString();
            lblTotalSize.Text = ByteSize.FromBytes(size).ToString("0.000 GB");
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("EXPORT.txt");
            file.WriteLine("Number of files: " + lblFileCount.Text);
            file.WriteLine("Total size of files: " + ByteSize.FromBytes(size).ToString("0.000 GB"));
            file.WriteLine();

            int max_length = 0;
            foreach (ListViewItem item in lstTorrents.Items)    //Determine the longest file name's length to place far enough the size data from the name data
            {
                int length = (item.SubItems[0].Text + item.SubItems[1].Text).Length;
                if (length > max_length)
                    max_length = length;
            }

            foreach (ListViewItem item in lstTorrents.Items)
            {
                int remaining = max_length - item.SubItems[0].Text.Length - item.SubItems[1].Text.Length + 4;
                file.WriteLine(item.SubItems[0].Text + new String('-', remaining) + item.SubItems[1].Text);
            }

            file.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lblFileCount.Text = "0";
            lblTotalSize.Text = "0 GB";
            size = 0;
            lstTorrents.Items.Clear();
        }
    }
}
