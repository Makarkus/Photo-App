using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMPImage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMPImage.Tests
{
    [TestClass()]
    public class Form1Tests
    {
        [TestMethod()]
        public void OpenToolStripMenuItem_ClickTest()
        {
            if(this.openFileDialog1.ShowDialog() == DialogResult.OK);
            Assert.Fail();
        }
        [TestMethod()]
        public void RunProccessing(Bitmap bitmap)
        {
            var expected = (bitmap.Width * bitmap.Height) / 100;
            Assert.Fail();
        }
    }
}