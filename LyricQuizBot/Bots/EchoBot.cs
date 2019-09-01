// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.5.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LyricQuizBot.QuestionGenerator;
using LyricQuizBot.State;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace LyricQuizBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private HeroCard HeroCard;
        private readonly BotState _convoState;
        private readonly BotState _userAnswer;

        public EchoBot(ConversationState convoState, UserState userState)
        {
            _convoState = convoState;
            _userAnswer = userState;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationStateAccessor = _convoState.CreateProperty<ConvoState>(nameof(ConvoState));
            var convoFlow = await conversationStateAccessor.GetAsync(turnContext, () => new ConvoState());

            var userStateAccessor = _userAnswer.CreateProperty<UserAnswers>(nameof(UserAnswers));
            var userProp = await userStateAccessor.GetAsync(turnContext, () => new UserAnswers());

            if(turnContext.Activity.Text.ToLower().Equals("mulai kuis") && !convoFlow.IsOnConvo)
            {
                convoFlow.IsOnConvo = true;
            }

            await Quiz(convoFlow, userProp, turnContext);

            await _convoState.SaveChangesAsync(turnContext);
            await _userAnswer.SaveChangesAsync(turnContext);
        }

        protected async Task Quiz(ConvoState convoState, UserAnswers userAnswers,ITurnContext turnContext)
        {
            string input = turnContext.Activity.Text;

            if (convoState.IsOnConvo)
            {
                switch (convoState.LastConvState)
                {
                    case ConvoState.ConversationState.None:
                        var quizMenu = MessageFactory.Text("Ingin Kuis Mode Apa?");
                        quizMenu.SuggestedActions = new SuggestedActions
                        {
                            Actions = new List<CardAction>()
                            {
                                new CardAction {Title = "Tebak Lagu", Type = ActionTypes.ImBack,Value="Tebak lagu"},
                                new CardAction {Title = "Lengkapi lirik", Type = ActionTypes.ImBack,Value="Lengkapi lirik"},
                            },
                        };
                        await turnContext.SendActivityAsync(quizMenu);
                        convoState.LastConvState = ConvoState.ConversationState.Menu;
                        break;
                    case ConvoState.ConversationState.Menu:
                        if(input.Equals("Tebak lagu"))
                        {
                            convoState.LastConvState = ConvoState.ConversationState.TebakLirikQuestiion;
                            await turnContext.SendActivityAsync("Ketik OK kalau kamu sudah siap");
                        }
                        else if(input.Equals("Lengkapi lirik"))
                        {
                            convoState.LastConvState = ConvoState.ConversationState.IsiLirikQuestion;
                            await turnContext.SendActivityAsync("Ketik OK kalau kamu sudah siap");
                        }
                        else if (input.Equals("Mulai kuis"))
                        {
                            var menu = MessageFactory.Text("Ingin Kuis Mode Apa?");
                            menu.SuggestedActions = new SuggestedActions
                            {
                                Actions = new List<CardAction>()
                                {
                                    new CardAction {Title = "Tebak Lagu", Type = ActionTypes.ImBack,Value="Tebak lagu"},
                                    new CardAction {Title = "Lengkapi lirik", Type = ActionTypes.ImBack,Value="Lengkapi lirik"},
                                },
                            };
                            await turnContext.SendActivityAsync(menu);
                            convoState.LastConvState = ConvoState.ConversationState.Menu;
                        }

                        break;
                    case ConvoState.ConversationState.IsiLirikQuestion:

                        SongMeta song = GetHardcodedSongList()[(new Random()).Next(GetHardcodedSongList().Count)];
                        convoState.QuestionItemNumber++;
                        if (convoState.QuestionItemNumber < 6)
                        {
                            if (convoState.QuestionItemNumber > 1)
                            {
                                if (input.ToLower().Equals(convoState.LastQuestionAsked.LyricAnswer))
                                {
                                    userAnswers.CorrectAnswer++;
                                    await turnContext.SendActivityAsync("Benaar!!");
                                }
                                else
                                {
                                    await turnContext.SendActivityAsync("Jawabanmu Salah");
                                }
                                if (convoState.QuestionItemNumber < 5)
                                {
                                    await turnContext.SendActivityAsync("Pertanyaan selanjutnya...");
                                }

                            }
                            else
                            {
                                await turnContext.SendActivityAsync("Ok bersiaplah. ");
                                await turnContext.SendActivityAsync("Pertanyaan pertama");
                            }
                            CompleteTheLyricQuestion question = LyricGenerator.Instance.GenerateCompleteTheLyricQuestion(song.Artist, song.Song);
                            convoState.LastQuestionAsked = question;
                            await turnContext.SendActivityAsync(question.LyricQuestion);
                            await turnContext.SendActivityAsync("Isilah kata pada lirik yang hilang");

                        }
                        else
                        {
                            if (input.ToLower().Equals(convoState.LastQuestionAsked.LyricAnswer.ToLower()))
                            {
                                userAnswers.CorrectAnswer++;
                                await turnContext.SendActivityAsync("Benaar!!");
                            }
                            else
                            {
                                await turnContext.SendActivityAsync("Jawabanmu Salah");
                            }
                            await turnContext.SendActivityAsync("Kuis sudah selesai");
                            await turnContext.SendActivityAsync($"Kamu menjawab benar sebanyak {userAnswers.CorrectAnswer} dari {convoState.QuestionItemNumber-1}");
                            convoState.LastConvState = ConvoState.ConversationState.Result;
                        }
                        break;
                    case ConvoState.ConversationState.TebakLirikQuestiion:
                        break;
                    case ConvoState.ConversationState.Result:
                        break;
                }
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var greetCard = new HeroCard
                    {
                        Title = "Selamat Datang di Kuis Lirik",
                        Buttons = new List<CardAction> { new CardAction { Title="Mulai kuis",Type = ActionTypes.ImBack,Value = "Mulai kuis"} }
                    };
                    var message = MessageFactory.Attachment(greetCard.ToAttachment());
                    await turnContext.SendActivityAsync(message);
                }
            }
        }

        private List<SongMeta> GetHardcodedSongList()
        {
            List<SongMeta> songs = new List<SongMeta>();
            songs.Add(new SongMeta { Artist = "Fiersa Besari", Song = "Celengan Rindu" });
            songs.Add(new SongMeta { Artist = "Fiersa Besari", Song = "Waktu Yang Salah" });
            songs.Add(new SongMeta { Artist = "Sheila On 7", Song = "Hari Bersamanya" });
            songs.Add(new SongMeta { Artist = "Sheila On 7", Song = "Dan" });
            songs.Add(new SongMeta { Artist = "Gita Gutawa", Song = "Bukan Permainan" });
            songs.Add(new SongMeta { Artist = "Sheila On 7", Song = "Seberapa Pantas" });
            songs.Add(new SongMeta { Artist = "Sheila On 7", Song = "Anugerah terindah yang pernah kumiliki" });
            songs.Add(new SongMeta { Artist = "Sheila On 7", Song = "Film Favorit" });
            songs.Add(new SongMeta { Artist = "Sheila On 7", Song = "Kita" });
            songs.Add(new SongMeta { Artist = "Sheila On 7", Song = "Pejantan Tangguh" });
            songs.Add(new SongMeta { Artist = "Via Vallen", Song = "Sayang" });
            songs.Add(new SongMeta { Artist = "Nella Kharisma", Song = "Bojo Galak" });
            songs.Add(new SongMeta { Artist = "Tulus", Song = "Monokrom" });
            songs.Add(new SongMeta { Artist = "Tulus", Song = "Sepatu" });
            songs.Add(new SongMeta { Artist = "Tulus", Song = "Manusia Kuat" });
            songs.Add(new SongMeta { Artist = "Tulus", Song = "Sepatu" });
            songs.Add(new SongMeta { Artist = "Adele", Song = "All I Ask" });
            songs.Add(new SongMeta { Artist = "Ten 2 Five", Song = "I Will Fly" });

            return songs;
        }
    }
}
