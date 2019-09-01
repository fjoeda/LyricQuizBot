using AzLyricsSharpApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LyricQuizBot.QuestionGenerator
{
    public class LyricGenerator
    {
        private static LyricGenerator _instance;

        public static LyricGenerator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LyricGenerator();
                return _instance;
            }
        }

        private LyricGenerator() { }

        private List<string> GetLyricsPerLines(string artist, string song)
        {
            List<string> lyricLines = new List<string>();
            AzLyrics lyric = new AzLyrics(artist, song);
            var lines = (lyric.GetLyris()).Split(new[] { '\r', '\n' });
            for (int i = 11; i < lines.Length; i++)
            {
                if (!lines[i].Equals(""))
                    lyricLines.Add(lines[i]);
            }
            return lyricLines;
        }

        private string GenerateQuestionFromLyric(List<string> lyricSnipLines, out string answer)
        {
            int randomLines = (new Random()).Next(lyricSnipLines.Count);
            string[] lyricWords = lyricSnipLines[randomLines].Split(" ");
            int chosenAnswerIndex = (new Random()).Next(lyricWords.Length);
            answer = lyricWords[chosenAnswerIndex];
            lyricWords[chosenAnswerIndex] = "(........)";
            string combinedwords = "";
            foreach(string words in lyricWords)
            {
                combinedwords += words + " ";
            }
            lyricSnipLines[randomLines] = combinedwords;
            string combinedLines = "";
            foreach (string lines in lyricSnipLines)
                combinedLines += lines + Environment.NewLine;
            return combinedLines;
        }

        public CompleteTheLyricQuestion GenerateCompleteTheLyricQuestion(string artist, string song)
        {
            List<string> lyricLines = GetLyricsPerLines(artist, song);
            List<string> lyricSnippetLines = new List<string>();
            CompleteTheLyricQuestion question = new CompleteTheLyricQuestion()
            {
                Artist = artist,
                Song = song,
                LyricSnippet = ""
            };
            int startLine = (new Random()).Next(lyricLines.Count - 6);
            for(int i = startLine; i < startLine + 4; i++)
            {
                question.LyricSnippet += lyricLines[i] + Environment.NewLine;
                lyricSnippetLines.Add(lyricLines[i]);
            }
            string answer;
            question.LyricQuestion = GenerateQuestionFromLyric(lyricSnippetLines, out answer);
            question.LyricAnswer = answer;
            return question;
        }
    }

    public class CompleteTheLyricQuestion
    {
        public string LyricSnippet { get; set; }
        public string LyricQuestion { get; set; }
        public string LyricAnswer { get; set; }
        public string Artist { get; set; }
        public string Song { get; set; }
    }

    public class GuessTheSongQuestion
    {
        public string LyricSnippet { get; set; }
        public string Artist { get; set; }
        public string Song { get; set; }
    }
}
