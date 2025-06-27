// using: 외부 네임스페이스 import
using System;
using System.Net.WebSockets;

namespace cs_basics
{
    // class ( 모든 C# 코드는 클래스 안에 있어야 함)
    public class Program
    {
        // Main Method (C# 실행 시작 위치)
        public static void Main(String[] args)
        {
            // varible types
            int age = 10; // 정수형 변수
            double pi = 3.14; // 실수형 변수
            string message = "Hello, World!"; // 문자열 변수
            bool isActive = true; // 불리언 변수

            Console.WriteLine($"Hi, {message}. Age: {age}");

            // if statement
            if (isActive) Console.WriteLine(" {} 없이 한줄 if 문 사용 가능");

            // for loop
            for (int i = 0; i <3; i++) Console.WriteLine($"for loop test print i: {i}");
            
        }
    }
}


