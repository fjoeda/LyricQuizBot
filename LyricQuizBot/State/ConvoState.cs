using LyricQuizBot.QuestionGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LyricQuizBot.State
{
    public class ConvoState
    {
        public enum ConversationState
        {
            None,
            Start,
            Menu,
            IsiLirikQuestion,
            TebakLirikQuestiion,
            Result
        }
        public ConversationState LastConvState { get; set; } = ConversationState.None;
        public CompleteTheLyricQuestion LastQuestionAsked { get; set; }
        public int QuestionItemNumber { get; set; } = 0;
        public bool IsOnConvo { get; set; } = false;
    }
}
