//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BasicCRM.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Level
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Level()
        {
            this.Bunches = new HashSet<Bunch>();
            this.Tests = new HashSet<Test>();
        }
    
        public int LevelID { get; set; }
        public string LevelName { get; set; }
        public Nullable<int> LessonID { get; set; }
        public Nullable<int> LevelTypeID { get; set; }
        public string LevelPrefix { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bunch> Bunches { get; set; }
        public virtual Lesson Lesson { get; set; }
        public virtual LevelType LevelType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Test> Tests { get; set; }
    }
}
