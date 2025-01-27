using MultiplicationChallenge.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MultiplicationChallenge
{
    public partial class MainWindow : Window
    {
        Random random = new Random();
        List<int> selectedNumbers = new List<int>();
        int correctAnswers = 0;
        int wrongAnswers = 0;

        int currentQuestionIndex = 0;
        List<Question> questions = new List<Question>();
        MediaPlayer player = new();
        MediaPlayer backgroundPlayer = new();



        public MainWindow()
        {
            InitializeComponent();
        }


        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            chooseQue.Visibility = Visibility.Visible;
            TblNum.IsEnabled = false;
            StartQuizBtn.Visibility = Visibility.Collapsed;
            AnswerBtn1.IsEnabled = AnswerBtn2.IsEnabled = AnswerBtn3.IsEnabled = AnswerBtn4.IsEnabled = true;


            selectedNumbers.Clear();

            if (AllNusCheck.IsChecked == true)
            {
                for (int i = 1; i <= 12; i++)
                {
                    selectedNumbers.Add(i);
                }
            }
            else
            {
                selectedNumbers.Add((int)MultiplierSlider.Value);
            }

            currentQuestionIndex = 0;
            questions.Clear();

            while (questions.Count < 10)
            {
                int firstNumber = selectedNumbers[random.Next(selectedNumbers.Count)];
                int secondNumber = random.Next(1, 13);

                if (!questions.Any(q => q.FirstNumber == firstNumber && q.SecondNumber == secondNumber))
                {
                    questions.Add(new Question(firstNumber, secondNumber));
                }
            }

            correctAnswers = 0;
            wrongAnswers = 0;
            CorrectCount.Text = "0";
            WrongCount.Text = "0";

            pgProgress.Maximum = questions.Count;
            pgProgress.Value = 0;

            LoadNextQuestion();
        }

        private void LoadNextQuestion()
        {
            if (currentQuestionIndex < questions.Count)
            {
                var currentQuestion = questions[currentQuestionIndex];
                QuestionLabel.Content = $"ما هو {currentQuestion.FirstNumber} × {currentQuestion.SecondNumber}؟";

                int correctAnswer = currentQuestion.GetCorrectAnswer();

                HashSet<int> options = new HashSet<int> { correctAnswer };

                while (options.Count < 4)
                {
                    int randomOption = correctAnswer + random.Next(-10, 11);
                    if (randomOption != correctAnswer && randomOption > 0)
                    {
                        options.Add(randomOption);
                    }
                }

                List<int> shuffledOptions = options.OrderBy(x => random.Next()).ToList();

                AnswerBtn1.Content = shuffledOptions[0];
                AnswerBtn2.Content = shuffledOptions[1];
                AnswerBtn3.Content = shuffledOptions[2];
                AnswerBtn4.Content = shuffledOptions[3];

                currentQuestionIndex++;

                pgProgress.Value = currentQuestionIndex;
            }
            else
            {
                SeccessDialog dialog = new SeccessDialog();

                PlaySound("Sounds/winner.wav");
                dialog.txtShowCorrect.Text = $"الإجابات الصحيحة: ({correctAnswers}).";
                dialog.txtShowWrong.Text = $"الإجابات الخاطئة: ({wrongAnswers}).\n\nقم بإعادة تعين الاختبار وابدأ الحل🤗";

                dialog.ShowDialog();

                TblNum.IsEnabled = true;
                StartQuizBtn.Visibility = Visibility.Visible;

                AnswerBtn1.IsEnabled = AnswerBtn2.IsEnabled = AnswerBtn3.IsEnabled = AnswerBtn4.IsEnabled = false;

                txtFeedback.Text = "تم الانتهاء من الاختبار! الرجاء إعادة البدء.";
                txtFeedback.Foreground = Brushes.Green;
            }
        }



        private void PlaySound(string soundPath)
        {
            try
            {
                player.Open(new Uri(soundPath, UriKind.Relative));
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في الصوت: {ex.Message}");
            }
        }

        private void PlayBackgroundMusic(string musicPath)
        {
            try
            {
                backgroundPlayer.Open(new Uri(musicPath, UriKind.Relative));
                backgroundPlayer.Volume = 0.1;
                backgroundPlayer.MediaEnded += (sender, e) =>
                {
                    backgroundPlayer.Position = TimeSpan.Zero;
                    backgroundPlayer.Play();
                };
                backgroundPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تشغيل الموسيقى الخلفية: {ex.Message}");
            }
        }


        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int selectedAnswer = int.Parse(button.Content.ToString());

            var currentQuestion = questions[currentQuestionIndex - 1];
            int correctAnswer = currentQuestion.GetCorrectAnswer();

            if (selectedAnswer == correctAnswer)
            {
                correctAnswers++;
                txtFeedback.Text = "إجابة صحيحة!";
                txtFeedback.Foreground = System.Windows.Media.Brushes.Green;
                PlaySound("Sounds/correct.wav");
            }
            else
            {
                wrongAnswers++;
                txtFeedback.Text = $"إجابة خاطئة! الإجابة الصحيحة: {correctAnswer}";
                txtFeedback.Foreground = System.Windows.Media.Brushes.Red;
                PlaySound("Sounds/wrong.wav");
            }

            CorrectCount.Text = correctAnswers.ToString();
            WrongCount.Text = wrongAnswers.ToString();

            LoadNextQuestion();
        }


        private void MultiplierSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtshowSlid != null)
            {
                txtshowSlid.Text = MultiplierSlider.Value.ToString("0");
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chooseQue.Visibility = Visibility.Collapsed;
            TblNum.IsEnabled = true;

            PlayBackgroundMusic("Sounds/bgSound.wav");
        }


        private void minApp_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void exitApp_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Normal)
                    WindowState = WindowState.Maximized;

                else
                    WindowState = WindowState.Normal;
            }
            else this.DragMove();
        }

        private bool isMusicPlaying = true;

        private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isMusicPlaying)
            {
                backgroundPlayer.Pause();
                isMusicPlaying = false;
            }
            else
            {
                backgroundPlayer.Play();
                isMusicPlaying = true;
            }
        }

    }
}