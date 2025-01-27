using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplicationChallenge.Classes
{
    public class Question
    {
        public int FirstNumber { get; set; }
        public int SecondNumber { get; set; }

        public Question(int firstNumber, int secondNumber)
        {
            FirstNumber = firstNumber;
            SecondNumber = secondNumber;
        }

        public int GetCorrectAnswer()
        {
            return FirstNumber * SecondNumber;
        }
    }
}
