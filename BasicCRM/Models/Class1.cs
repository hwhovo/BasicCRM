using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasicCRM.Models
{
    public class TestItem
        {
        public TestsArchive testArchive { get; set; }

        public ICollection<QuestionItem> Questions { get; set; }

}

    public class QuestionItem
    {

        public Question question { get; set; }

        public ICollection<AnswerItem> Answers { get; set; }
    }

    public class AnswerItem
    {
        [Key]
        public int AnswerItemID { get; set; }
        public Answer answer { get; set; }

        public bool IsChecked { get; set; }

    }
}