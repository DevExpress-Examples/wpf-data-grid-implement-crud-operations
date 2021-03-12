using System;
using System.Linq;

namespace DevExpress.CRUD.ViewModel {
    public class EntityUpdateArgs {
        public EntityUpdateArgs(object entity) {
            Entity = entity;
        }
        public object Entity { get; }
        public bool Updated { get; set; }
    }
}
