using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LyricQuizBot.State
{
    public class UserAnswers
    {
        public List<bool> IsQuestionCorrect { get; set; } = new List<bool>();
        public int CorrectAnswer { get; set; }
    }

    public class SongMeta
    {
        public string Artist { get; set; }
        public string Song { get; set; }
    }
}
