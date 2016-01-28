using System;
using System.Collections.Generic;
using System.Text;

using WW.Cad.Model.Entities;

namespace WinFormsEditExample {
    public class EntityEventArgs : EventArgs {
        DxfEntity entity;

        public EntityEventArgs(DxfEntity entity) {
            this.entity = entity;
        }

        public DxfEntity Entity {
            get {
                return entity;
            }
        }
    }
}
