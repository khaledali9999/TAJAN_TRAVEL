using System.ComponentModel.DataAnnotations;

namespace TAJAN_TRAVEL.Models.Entities
{
    public class FAQ
    {
        public int Id { get; set; }

        [Required, MaxLength(250)]
        public string QuestionEn { get; set; } = string.Empty;

        [Required, MaxLength(250)]
        public string QuestionFr { get; set; } = string.Empty;

        [Required]
        public string AnswerEn { get; set; } = string.Empty;

        [Required]
        public string AnswerFr { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? Category { get; set; }

        [MaxLength(80)]
        public string? CategoryFr { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        // ---- culture aware (English / French) - used by the public views ----
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Question => TAJAN_TRAVEL.Localization.Lang.Pick(QuestionEn, QuestionFr);

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Answer => TAJAN_TRAVEL.Localization.Lang.Pick(AnswerEn, AnswerFr);
    }
}
