using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Models {
    public class MenuModel {
        public List<BEntities.Base.Menu> Items { get; set; }
        public long Id { get; set; }
        public int Level { get; set; }

        public MenuModel(List<BEntities.Base.Menu> Items, long Id, int Level) {
            this.Items = Items;
            this.Id = Id;
            this.Level = Level;
        }
    }
}
