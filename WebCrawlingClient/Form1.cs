using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Runtime.InteropServices;

using System.IO;

namespace WebCrawlingClient
{
    public partial class Form1 : Form
    {
        struct article
        {
            string headline;
            string description;
            string author;
            string dateOfPublication;
            string url;
        }
        Queue<String> UrlList = new Queue<string>();

        public Form1()
        {
            InitializeComponent();
            
        }

        private void ControlBoxLabel_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Take URL from URLBox and add it to URL list file
            //potentially add URL checking
            using (StreamWriter w = File.AppendText("UrlList.txt"))
            {
                w.WriteLine(URLBox.Text);
                w.Close();
            }
            URLBox.Text = "";
           
        }

        private void BeginCrawl_Click(object sender, EventArgs e)
        {
            URLLabel1.Text = UrlList.ElementAt(0);
            //washingtonPost appears to start headlines with "headline":" each time
            //begin crawl of first three items in the list
            int headerCharLength = 12;
            int descriptionCharLength = 15;
            int authorCharLength = 10;
            foreach(string urlItem in UrlList)
            {
                //Temporary Variables to hold all item information
                string tempHeader = "", tempDescription = "", tempAuthor = "", tempPubDate = "";

                var client = new WebClient();
                var text = client.DownloadString(urlItem);
                //FIND HEADER:

                var indexOfHeaderTag = text.LastIndexOf(@"" + '\u0022' + "headline" + '\u0022' + ":" + '\u0022');
                //this index ^ is last index of the beginning of the headline title we should search until we reach another " which is represented by
                //unicode \u0022
                //12 is the size of the headline label in source code
                
                int indexForHeaderGet = indexOfHeaderTag + headerCharLength;
                char curChar = text[indexForHeaderGet];

                while (curChar != '\u0022')
                {
                    tempHeader = tempHeader + curChar.ToString();
                    indexForHeaderGet++;
                    curChar = text[indexForHeaderGet];

                }
                Console.WriteLine(tempHeader);
                textToDisplay1.Text = tempHeader;

                //FIND DESCRIPTION
                var indexOfDescriptionTag = text.IndexOf(@"" + '\u0022' + "description" + '\u0022' + ":" + '\u0022');
                int indexForDescriptionGet = indexOfDescriptionTag + descriptionCharLength;
                curChar = text[indexForDescriptionGet];

                while(curChar != '\u0022')
                {                    
                    tempDescription = tempDescription + curChar.ToString();

                    
                    indexForDescriptionGet++;
                    curChar = text[indexForDescriptionGet];
                }
                Console.WriteLine(tempDescription);
                Description1.Text = tempDescription;

                //Finding the Author
                var indexofAuthorTag = text.IndexOf(@"" + '\u0022' + "author" + '\u0022' + ":" + '\u0022');
                int indexForAuthorGet = indexofAuthorTag + authorCharLength;
                curChar = text[indexForAuthorGet];
                while(curChar != '\u0022')
                {
                    tempAuthor = tempAuthor + curChar.ToString();
                    indexForAuthorGet++;
                    curChar = text[indexForAuthorGet];
                }
                Console.WriteLine(tempAuthor);
                AuthorLabel1.Text = tempAuthor;
            }


        }

        private void ClearFileBtn_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllText(@"UrlList.txt", string.Empty);
            URLListBox.Items.Clear();
        }

        private void URLListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void URLListBox_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(URLListBox.GetItemText(URLListBox.SelectedItem));

        }

        private void ListRefreshBtn_Click(object sender, EventArgs e)
        {
            URLListBox.Items.Clear();
            string line;
            // Read the file and display it line by line.  
            //first get all of the URLs from the URL List File
            System.IO.StreamReader file =
                new System.IO.StreamReader(@"UrlList.txt");

            while ((line = file.ReadLine()) != null)
            {
                UrlList.Enqueue(line);
                URLListBox.Items.Add(line);
            }
            file.Close();
        }
    }
}
