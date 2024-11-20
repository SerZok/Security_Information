#define BIT
#define WEAK
using System.Text;



internal class Programm
{
    public static void Main()
    {
        //string KEY = "616c656b6f73";
        string keyStr = "25df 32ac 2473 dea2";
        string inputStr = "Hello world! IVT-42-21 Zaytsev Sergey A";
        string inputEncr = DES.GetBlock(inputStr);
        Console.WriteLine($"ОТ: \"{inputStr}\"");

        DES.GetKeys(keyStr);
#if BIT
        string strRes1 = "";
         Console.WriteLine("Шифрограмма: ");
        foreach (string keyString in DES.inTextBlocks)
        {
            bool[] bitArray = DES.StringToBitArray(keyString);
            bool[] encodeBool = DES.Encode(bitArray);
            DES.outBoolBlocks.Add(encodeBool);
            string encodeStr = DES.ConvertBoolArrayToString(encodeBool);
            DES.outTextBlocks.Add(encodeStr);
            strRes1 += encodeStr;
            Console.WriteLine($"{encodeStr}");
        }
        Console.WriteLine(strRes1);
#else
        Console.WriteLine("Шифрограмма с искаженным битом: ");
        string strRes1 = "";
        foreach (string keyString in DES.inTextBlocks)
        {
            bool[] bitArray = DES.StringToBitArray(keyString);
            bool[] encodeBool = DES.Encode(bitArray);
            DES.outBoolBlocks.Add(encodeBool);
            string encodeStr = DES.ConvertBoolArrayToString(encodeBool);
            DES.outTextBlocks.Add(encodeStr);
            Console.WriteLine($"{encodeStr}");
            strRes1 += encodeStr;
        }
        DES.outBoolBlocks[0][0] = !DES.outBoolBlocks[0][0];
        Console.WriteLine(strRes1);
#endif

        

#if WEAK
        Console.WriteLine("Расшифрованная строка со слабым ключом, путем повторного шифрования: ");
        string resDES = "";
        foreach (bool[] keyBool in DES.outBoolBlocks)
        {
            bool[] decodeBool = DES.Encode(keyBool);
            string decodeStr = DES.ConvertBoolArrayToString(decodeBool);
            Console.WriteLine($"{decodeStr}");
            resDES += decodeStr;
        }
        Console.WriteLine(resDES);

#else
        //Расшифровка
        Console.WriteLine("\nРасшифровка: ");
        string strRes2 = "";
        foreach (bool[] keyBool in DES.outBoolBlocks)
        {
            string decodeStr = DES.Decode(keyBool);
            Console.WriteLine($"{decodeStr}");
            strRes2 += decodeStr;
        };
        Console.WriteLine(strRes2);

#endif
    }
}

public class DES
{
    //16 - сгенерированных ключей
    public static List<bool[]> keys = new List<bool[]>();
    public static List<string> inTextBlocks = new List<string>();
    public static List<bool[]> outBoolBlocks = new List<bool[]>();
    public static List<string> outTextBlocks = new List<string>();

    private static readonly int[] IP = new int[64]
    {
        57,  49,  41,  33,  25,  17,  9,   1,
        59,  51,  43,  35,  27,  19,  11,  3,
        61,  53,  45,  37,  29,  21,  13,  5,
        63,  55,  47,  39,  31,  23,  15,  7,
        56,  48,  40,  32,  24,  16,  8,   0,
        58,  50,  42,  34,  26,  18,  10,  2,
        60,  52,  44,  36,  28,  20,  12,  4,
        62,  54,  46,  38,  30,  22,  14,  6
    };

    private static readonly int[] PC1 = new int[56]
    {
        56, 48, 40, 32, 24, 16, 8,  0,
        57, 49, 41, 33, 25, 17, 9,  1,
        58, 50, 42, 34, 26, 18, 10, 2,
        59, 51, 43, 35, 62, 54, 46, 38,
        30, 22, 14, 6,  61, 53, 45, 37,
        29, 21, 13, 5,  60, 52, 44, 36,
        28, 20, 12, 4,  27, 19, 11, 3
    };

    private static readonly int[] PC2 = new int[48]
    {
        13, 16, 10, 23, 0,  4,  2,  27,
        14, 5,  20, 9,  22, 18, 11, 3,
        25, 7,  15, 6,  26, 19, 12, 1,
        40, 51, 30, 36, 46, 54, 29, 39,
        50, 44, 32, 47, 43, 48, 38, 55,
        33, 52, 45, 41, 49, 35, 28, 31
    };

    private static readonly int[] IP_1 = new int[64]
    {
        39,  7,  47,  15,  55,  23,  63,  31,
        38,  6,  46,  14,  54,  22,  62,  30,
        37,  5,  45,  13,  53,  21,  61,  29,
        36,  4,  44,  12,  52,  20,  60,  28,
        35,  3,  43,  11,  51,  19,  59,  27,
        34,  2,  42,  10,  50,  18,  58,  26,
        33,  1,  41,  9,   49,  17,  57,  25,
        32,  0,  40,  8,   48,  16,  56,  24
    };

    private static readonly int[] E = new int[48]
    {
        31,  0,   1,   2,   3,   4,
        3,   4,   5,   6,   7,   8,
        7,   8,   9,   10,  11,  12,
        11,  12,  13,  14,  15,  16,
        15,  16,  17,  18,  19,  20,
        19,  20,  21,  22,  23,  24,
        23,  24,  25,  26,  27,  28,
        27,  28,  29,  30,  31,  0
    };

    private static readonly int[] SBlocks = new int[512] {
        14,  4, 13,  1,  2, 15, 11,  8,  3, 10,  6, 12,  5,  9,  0,  7,
         0, 15,  7,  4, 14,  2, 13,  1, 10,  6, 12, 11,  9,  5,  3,  8,
         4,  1, 14,  8, 13,  6,  2, 11, 15, 12,  9,  7,  3, 10,  5,  0,
        15, 12,  8,  2,  4,  9,  1,  7,  5, 11,  3, 14, 10,  0,  6, 13,

        15,  1,  8, 14,  6, 11,  3,  4,  9,  7,  2, 13, 12,  0,  5, 10,
         3, 13,  4,  7, 15,  2,  8, 14, 12,  0,  1, 10,  6,  9, 11,  5,
         0, 14,  7, 11, 10,  4, 13,  1,  5,  8, 12,  6,  9,  3,  2, 15,
        13,  8, 10,  1,  3, 15,  4,  2, 11,  6,  7, 12,  0,  5, 14,  9,

        10,  0,  9, 14,  6,  3, 15,  5,  1, 13, 12,  7, 11,  4,  2,  8,
        13,  7,  0,  9,  3,  4,  6, 10,  2,  8,  5, 14, 12, 11, 15,  1,
        13,  6,  4,  9,  8, 15,  3,  0, 11,  1,  2, 12,  5, 10, 14,  7,
         1, 10, 13,  0,  6,  9,  8,  7,  4, 15, 14,  3, 11,  5,  2, 12,

         7, 13, 14,  3,  0,  6,  9, 10,  1,  2,  8,  5, 11, 12,  4, 15,
        13,  8, 11,  5,  6, 15,  0,  3,  4,  7,  2, 12,  1, 10, 14,  9,
        10,  6,  9,  0, 12, 11,  7, 13, 15,  1,  3, 14,  5,  2,  8,  4,
         3, 15,  0,  6, 10,  1, 13,  8,  9,  4,  5, 11, 12,  7,  2, 14,

         2, 12,  4,  1,  7, 10, 11,  6,  8,  5,  3, 15, 13,  0, 14,  9,
        14, 11,  2, 12,  4,  7, 13,  1,  5,  0, 15, 10,  3,  9,  8,  6,
         4,  2,  1, 11, 10, 13,  7,  8, 15,  9, 12,  5,  6,  3,  0, 14,
        11,  8, 12,  7,  1, 14,  2, 13,  6, 15,  0,  9, 10,  4,  5,  3,

        12,  1, 10, 15,  9,  2,  6,  8,  0, 13,  3,  4, 14,  7,  5, 11,
        10, 15,  4,  2,  7, 12,  9,  5,  6,  1, 13, 14,  0, 11,  3,  8,
         9, 14, 15,  5,  2,  8, 12,  3,  7,  0,  4, 10,  1, 13, 11,  6,
         4,  3,  2, 12,  9,  5, 15, 10, 11, 14,  1,  7,  6,  0,  8, 13,

         4, 11,  2, 14, 15,  0,  8, 13,  3, 12,  9,  7,  5, 10,  6,  1,
        13,  0, 11,  7,  4,  9,  1, 10, 14,  3,  5, 12,  2, 15,  8,  6,
         1,  4, 11, 13, 12,  3,  7, 14, 10, 15,  6,  8,  0,  5,  9,  2,
         6, 11, 13,  8,  1,  4, 10,  7,  9,  5,  0, 15, 14,  2,  3, 12,

        13,  2,  8,  4,  6, 15, 11,  1, 10,  9,  3, 14,  5,  0, 12,  7,
         1, 15, 13,  8, 10,  3,  7,  4, 12,  5,  6, 11,  0, 14,  9,  2,
         7, 11,  4,  1,  9, 12, 14,  2,  0,  6, 10, 13, 15,  3,  5,  8,
         2,  1, 14,  7,  4, 10,  8, 13, 15, 12,  9,  0,  3,  5,  6, 11
    };

    private static readonly int[] P = new int[32]
    {
        15,  6,   19,  20,  28,  11,  27,  16,
        0,   14,  22,  25,  4,   17,  30,  9,
        1,   7,   23,  13,  31,  26,  2,   8,
        18,  12,  29,  5,   21,  10,  3,   24
    };

    private static readonly int[] LS = new int[16]
    {
        1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
    };
    public static bool[] StringToBitArray(string str)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(str);
        bool[] bits = new bool[64];

        int index = 0;
        foreach (byte b in bytes)
        {
            for (int i = 0; i < 8; i++)
            {
                if (index < 64)
                    bits[index++] = (b & (1 << (7 - i))) != 0;
            }
        }
        return bits;
    }
    public static bool[] StringHexToBitArray(string hexString)
    {
        // Удаляем возможные пробелы из строки
        hexString = hexString.Replace(" ", "").Trim();
        // Преобразуем шестнадцатеричную строку в массив байт
        int bytesCount = hexString.Length / 2;
        byte[] bytes = new byte[bytesCount];

        for (int i = 0; i < bytesCount; i++)
            bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

        // Создаем массив битов длиной 64
        bool[] bits = new bool[64];

        int index = 0;
        foreach (byte b in bytes)
        {
            for (int i = 0; i < 8; i++)
            {
                if (index < 64)
                    bits[index++] = (b & (1 << (7 - i))) != 0;
            }
        }

        while (index < 64)
        {
            bits[index++] = false;
        }

        return bits;
    }
    /// <summary>
    /// Начальная переставновка IP
    /// </summary>
    /// <param name="bits"></param>
    /// <returns></returns>
    public static bool[] BitArrayPermutationIP(bool[] bits)
    {
        bool[] bitsPermutation = new bool[64];
        for (int i = 0; i < 64; i++)
            bitsPermutation[i] = bits[IP[i]];
        return bitsPermutation;
    }
    /// <summary>
    /// Конечная перестановка IP-1
    /// </summary>
    /// <param name="bits"></param>
    /// <returns></returns>
    public static bool[] BitArrayPermutationIP_1(bool[] bits)
    {
        bool[] bitsPermutation = new bool[64];
        for (int i = 0; i < 64; i++)
            bitsPermutation[i] = bits[IP_1[i]];
        return bitsPermutation;
    }
    /// <summary>
    /// Перестановка ключей PC1
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool[] BitArrayPermutationKey(bool[] key)
    {
        bool[] bitsPermutationKey = new bool[56];
        for (int i = 0; i < 56; i++)
            bitsPermutationKey[i] = key[PC1[i]];
        return bitsPermutationKey;
    }
    public static bool[] XOR(bool[] s1, bool[] s2)
    {
        bool[] result = new bool[s1.Length];
        //Каждый бит
        for (int i = 0; i < s1.Length; i++)
            result[i] = s1[i] ^ s2[i];
        return result;
    }
    public static int BinaryToDecimal(string binary)
    {
        int decimalValue = 0;
        int baseValue = 1;

        for (int i = binary.Length - 1; i >= 0; i--)
        {
            if (binary[i] == '1')
                decimalValue += baseValue;
            baseValue *= 2;
        }

        return decimalValue;
    }
    public static bool[] DecimalToBinaryArray(int decimalNumber)
    {
        bool[] binaryArray = new bool[4];
        for (int i = 0; i < 4; i++)
            binaryArray[3 - i] = (decimalNumber & (1 << i)) != 0;
        return binaryArray;
    }
    public static bool[] CombineBoolArrays(bool[][] boolArrays)
    {
        bool[] combinedArray = new bool[32];
        int index = 0;

        for (int i = 0; i < boolArrays.Length; i++)
        {
            for (int j = 0; j < boolArrays[i].Length; j++)
                combinedArray[index++] = boolArrays[i][j];
        }

        return combinedArray;
    }
    /// <summary>
    /// Перестановки P
    /// </summary>
    /// <param name="bits"></param>
    /// <returns></returns>
    public static bool[] BitArrayPermutationP(bool[] bits)
    {
        bool[] bitsPermutation = new bool[32];
        for (int i = 0; i < 32; i++)
            bitsPermutation[i] = bits[P[i]];
        return bitsPermutation;
    }
    public static bool[] f(bool[] R, bool[] key)
    {
        // Расширение Е
        bool[] expansionMatrixE = new bool[48];
        for (int i = 0; i < 48; i++)
            expansionMatrixE[i] = R[E[i]];

        // Гаммирование
        bool[] gammingWithKey = XOR(expansionMatrixE, key);

        // Разделение на S блоки
        bool[][] block_S = new bool[8][];
        for (int i = 0; i < 8; i++)
        {
            block_S[i] = new bool[6]; // Инициализация каждого блока с 6 элементами
            Array.Copy(gammingWithKey, i * 6, block_S[i], 0, 6); // Копирование соответствующих элементов из исходного массива
        }

        // Результат S блоков
        bool[][] block_S_Output = new bool[8][];

        for (int i = 0; i < 8; i++)
        {
            string k_Binary = String.Empty;
            string l_Binary = String.Empty;

            k_Binary = $"{Convert.ToInt32(block_S[i][0])}{Convert.ToInt32(block_S[i][5])}";
            l_Binary = $"{Convert.ToInt32(block_S[i][1])}{Convert.ToInt32(block_S[i][2])}" + $"{Convert.ToInt32(block_S[i][3])}{Convert.ToInt32(block_S[i][4])}";

            int k = BinaryToDecimal(k_Binary);
            int l = BinaryToDecimal(l_Binary);


            int D = SBlocks[(64 * i) + (k * 16) + l];
            block_S_Output[i] = new bool[4];
            block_S_Output[i] = DecimalToBinaryArray(D);
        }

        bool[] combinedArray = CombineBoolArrays(block_S_Output);

        // Перестановка P
        bool[] bitArrayPermutationP = BitArrayPermutationP(combinedArray);

        return bitArrayPermutationP;
    }
    public static string ConvertBoolArrayToString(bool[] boolArray)
    {
        if (boolArray.Length != 64)
        {
            throw new ArgumentException("Array must contain exactly 64 elements.");
        }

        byte[] byteArray = new byte[8]; // Массив байтов для хранения 8 байтов (64 бита)

        for (int i = 0; i < 64; i += 8) // Проходим по массиву, по 8 бит (1 байт)
        {
            byte byteValue = 0;
            for (int j = 0; j < 8; j++)
            {
                if (boolArray[i + j])
                {
                    byteValue |= (byte)(1 << (7 - j)); // Устанавливаем соответствующий бит
                }
            }
            byteArray[i / 8] = byteValue; // Сохраняем байт в массив
        }

        return Encoding.ASCII.GetString(byteArray); // Конвертируем массив байтов в строку
    }
    public static void GetKeys(string keyStr)
    {
        keys.Clear();
        List<bool[]> readyMadeKeys = new List<bool[]>();

        bool[] key = StringHexToBitArray(keyStr);
        bool[] bitArrayPermutationKey = BitArrayPermutationKey(key);

        int halfLength = bitArrayPermutationKey.Length / 2;
        bool[] L = new bool[halfLength];
        bool[] R = new bool[halfLength];

        Array.Copy(bitArrayPermutationKey, 0, L, 0, halfLength);
        Array.Copy(bitArrayPermutationKey, halfLength, R, 0, halfLength);

        for (int i = 0; i < 16; i++)
        {
            bool[] oneKey = new bool[48];
            // Функция сдвига влево L и R
            ShiftLeft(L, LS[i]);
            ShiftLeft(R, LS[i]);

            // Объединение в один 56-битовый массив
            bool[] mergedLR = new bool[L.Length + R.Length];
            L.CopyTo(mergedLR, 0);
            R.CopyTo(mergedLR, L.Length);

            // Перествноквка-выбор PC2
            for (int j = 0; j < 48; j++)
            {
                oneKey[j] = mergedLR[PC2[j]];
            }
            keys.Add(oneKey);
        }
    }
    public static void ShiftLeft(bool[] array, int n)
    {
        int length = array.Length;
        n = n % length;

        // Создаем временный массив для хранения сдвинутых значений
        bool[] temp = new bool[length];

        // Копируем значения в новый массив с учетом сдвига
        for (int i = 0; i < length; i++)
            temp[i] = array[(i + n) % length];

        // Копируем значения обратно в оригинальный массив
        for (int i = 0; i < length; i++)
            array[i] = temp[i];
    }
    public static bool[] Encode(bool[] bitArray)
    {
        bool[] bitArrayPermutationIp = BitArrayPermutationIP(bitArray);
        int halfLength = bitArrayPermutationIp.Length / 2;
        bool[] L = new bool[halfLength];
        bool[] R = new bool[halfLength];
        Array.Copy(bitArrayPermutationIp, 0, L, 0, halfLength);
        Array.Copy(bitArrayPermutationIp, halfLength, R, 0, halfLength);

        for (int i = 0; i < 16; i++)
        {
            var temp = R;
            R = XOR(L, f(temp, keys[i]));
            L = temp;
        }

        bool[] mergedLR = new bool[L.Length + R.Length];
        R.CopyTo(mergedLR, 0);
        L.CopyTo(mergedLR, R.Length);

        return BitArrayPermutationIP_1(mergedLR);
    }
    public static string Decode(bool[] bitArray)
    {
        bool[] bitArrayPermutationIp = BitArrayPermutationIP(bitArray);

        int halfLength = bitArrayPermutationIp.Length / 2;
        bool[] L = new bool[halfLength];
        bool[] R = new bool[halfLength];
        Array.Copy(bitArrayPermutationIp, 0, L, 0, halfLength);
        Array.Copy(bitArrayPermutationIp, halfLength, R, 0, halfLength);

        for (int i = 15; i >= 0; i--)
        {
            bool[] temp = new bool[R.Length];
            R.CopyTo(temp, 0);
            R = XOR(L, f(temp, keys[i]));
            L = temp;
        }

        bool[] mergedLR = new bool[L.Length + R.Length];
        R.CopyTo(mergedLR, 0);
        L.CopyTo(mergedLR, R.Length);

        bool[] decodeBool = BitArrayPermutationIP_1(mergedLR);

        return ConvertBoolArrayToString(decodeBool);
    }
    /// <summary>
    /// Формирование 8-байтового блока из исходного текста
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string GetBlock(string input)
    {
        while (input.Length % 8 != 0)
        {
            input += " ";
        }
        string tempBlock = "";
        inTextBlocks.Clear();
        for (int i = 0; i < input.Length; i++)
        {
            if (i != 0 && i % 8 == 0 || i == input.Length - 1)
            {
                if (i == 7 && input.Length == 8)
                {
                    tempBlock += input[i];
                }
                inTextBlocks.Add(tempBlock);
                tempBlock = "";
            }
            tempBlock += input[i];
        }
        return input;
    }
}