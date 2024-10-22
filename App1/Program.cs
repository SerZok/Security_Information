using System;
using System.Collections.Generic;

class SubstitutionCipher
{
    // Алфавит
    static readonly char[] alphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ".ToCharArray();

    //Таблица подстановки
    static readonly Dictionary<char, char> substitutionTable = new Dictionary<char, char>
    {
        {'А', 'Г'}, {'Б', 'Ж'}, {'В', 'К'}, {'Г', 'Л'}, {'Д', 'Н'}, {'Е', 'Р'},
        {'Ё', 'С'}, {'Ж', 'Т'}, {'З', 'У'}, {'И', 'Ф'}, {'Й', 'Х'}, {'К', 'Ц'},
        {'Л', 'Ч'}, {'М', 'Ш'}, {'Н', 'Щ'}, {'О', 'Ъ'}, {'П', 'Ы'}, {'Р', 'Ь'},
        {'С', 'Э'}, {'Т', 'Ю'}, {'У', 'Я'}, {'Ф', 'А'}, {'Х', 'Б'}, {'Ц', 'В'},
        {'Ч', 'Д'}, {'Ш', 'Е'}, {'Щ', 'Ё'}, {'Ъ', 'Ж'}, {'Ы', 'З'}, {'Ь', 'И'},
        {'Э', 'Й'}, {'Ю', 'М'}, {'Я', 'О'}
    };

    static void Main()
    {
        Console.WriteLine("Введите текст для шифрования (только русские буквы):");
        string text = Console.ReadLine().ToUpper();

        string encryptedText = Encrypt(text);
        string decryptedText = Decrypt(encryptedText);

        Console.WriteLine($"Зашифрованный текст: {encryptedText}");
        Console.WriteLine($"Расшифрованный текст: {decryptedText}");
    }

    // Функция шифрования
    static string Encrypt(string plainText)
    {
        string cipherText = "";
        foreach (char ch in plainText)
        {
            if (substitutionTable.ContainsKey(ch))
                cipherText += substitutionTable[ch];
            else
                cipherText += ch;
        }
        return cipherText;
    }

    // Функция дешифрования
    static string Decrypt(string cipherText)
    {
        string plainText = "";
        foreach (char ch in cipherText)
        {
            char originalChar = GetOriginalChar(ch);
            plainText += originalChar;
        }
        return plainText;
    }

    // Поиск оригинального символа для дешифрования
    static char GetOriginalChar(char cipherChar)
    {
        foreach (var pair in substitutionTable)
        {
            if (pair.Value == cipherChar)
                return pair.Key;
        }
        return cipherChar; //Если не нашли то просто возварщаем
    }
}
