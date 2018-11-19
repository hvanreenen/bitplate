using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitPlate.Domain
{
    
    public class CmsImage
    {
        public int id {get; set;}
        public string name { get; set; }
        public int height {get; set;}
        public int width {get; set;}
        public int previewZoomWidth { get; set; }
        public int previewZoomHeight { get; set; }
        public int x {get; set;}
        public int y {get; set;}
        public int x2 {get; set;}
        public int y2 {get; set; }
        public bool aspectRatio { get; set; }
    }
}
