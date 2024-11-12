using System;
using System.Collections;
using System.Text;

namespace App4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string text = Console.ReadLine();
            //long weakKey1 = 72340172838076673;
            //long weakKey2 = 1837440390087147494;
            //long weakKey3 = 2242545357694045710;
            //long weakKey4 = 1620419871601550590;
            //long strongKey1 = 0x101010101010;
            string text = "eternity";
            Console.WriteLine($"Исходный текст: \"{text}\"");

            long keyLong = 0x616c656b6f73;   

            var res1 = DES.Encrypt(text, keyLong);
            var strRes1 = Encoding.ASCII.GetString(res1);
            Console.WriteLine($"Зашифрованный текст: \"{strRes1}\", Кол-во байт: {res1.Length}");

            

           var res2 = DES.Decrypt(res1, keyLong);
            foreach (var item in res2)
                Console.WriteLine(item);
            Console.WriteLine($"Расшифрованный текст: \"{res2}\",  Кол-во байт: {res2.Length}");


            for (int i=0; i< text.Length; i++)
            {
                Console.WriteLine($"ОТ: {text[i]} -> ШТ: {res1[i]} -> ОТ:{res2[i]}");
            }

        }

    }

    public class DES
    {
        //Таблица начальной перестановки (позиция битов)
        private static readonly int[] IP = {
            58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7
        };

        //Таблица обратной перестановки
        private static readonly int[] IP_INV = {
        40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31,
        38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29,
        36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27,
        34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25
        };

        //Таблица расширения (из 32 -> 48 )
        private static readonly int[] E = [
            32, 1, 2, 3, 4, 5,
            4, 5, 6, 7, 8, 9,
            8, 9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32, 1
        ];

        //Таблица перестановки P
        private static readonly int[] P = {
            16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26, 5, 18, 31, 10,
            2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22, 11, 4, 25
        };

        //Таблицы для S-блоков S1-S8
        private static readonly int[,,] S_BOXES = {
            {//1
                { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
                {0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
                {4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
                { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
            },
            {//2
                {15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10},
                {3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5},
                {0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15},
                { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14,9}
            },
            {//3
                {10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8},
                {13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1},
                {13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
                { 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 }
            },
            {//4
                {7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
                {13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
                {10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
                { 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
            },
            {//5
                {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9},
                {14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6},
                {4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
                { 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3}
            },
            {//6
                {12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
                {10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
                {9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
                { 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 },
            },
            {//7
                { 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
                {13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
                {1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
                {6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
            },
            {//8
                {13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
                {1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
                {7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 13, 5, 8 },
                { 2, 1, 14, 7, 4, 10, 8, 13, 5, 12, 9, 0, 3, 5, 5, 11 }
            }
        };

        //Начальная таблица перестановки ключа
        private static readonly int[] PC1 = {
            57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18,
            10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22,
            14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4,
        };

        //Таблица перестановки для генерации Ki
        private static readonly int[] PC2 = {
           14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4,
           26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40,
           51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32
        };

        //Таблица циклических сдвигов для ключа
        private static readonly int[] SHIFT_Key = {
             1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
        };

        public static byte[] Encrypt(string text, long key)
        {
            //1. Разбиение текста на блоки по 8 байт
            List<byte[]> bytes = To8Byte(text);

            byte[] byteArray = new byte[8 * bytes.Count];
            int bytePosition = 0;

            //Формирование ключей
            List<BitArray> Keys = CreateKeys(key);

            //Каждый блок по отдельности
            foreach (var Block8 in bytes)
            {
                BitArray Bits64 = new BitArray(Block8);

                // Изменение порядка битов в каждом блоке по 8 бит
                for (int i = 0; i < Bits64.Length; i += 8)
                {
                    bool[] reversedBlock = new bool[8];
                    for (int j = 0; j < 8; j++)
                    {
                        reversedBlock[j] = Bits64[i + (7 - j)]; // Переворачиваем порядок битов
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        Bits64[i + j] = reversedBlock[j]; // Записываем перевернутый блок обратно
                    }
                }

                //2. Начальная перестановка битов (IP)
                Bits64 = Swap(Bits64, IP);

                //Bits64 = Block8.CopyTo(Bits64, 0);

                BitArray L = new(32);
                BitArray R = new(32);
                CopyBaToLR(Bits64, L, R);

                //3. Циклы преобразований
                for (int i = 0; i < 16; i++)
                {
                    var oldR = R;
                    R = L.Xor(Func(R, Keys[i]));
                    L = oldR;
                }

                BitArray resBits = new BitArray(64);
                for (int i = 0; i < 32; i++)
                {
                    resBits[i] = R[i];
                    resBits[32 + i] = L[i];
                }

                //Обратная перестановка (IP_INV)
                resBits = Swap(resBits, IP_INV);

                resBits.CopyTo(byteArray, bytePosition);
                bytePosition += resBits.Length / 8;
            }
            //return Encoding.ASCII.GetString(byteArray);
            return byteArray;
        }

        public static string Decrypt(byte[] text, long key)
        {
            //1. Разбиение текста на блоки по 8 байт
            List<byte[]> bytes = To8Byte2(text);

            byte[] byteArray = new byte[8 * bytes.Count];
            int bytePosition = 0;

            //Формирование ключей
            List<BitArray> Keys = CreateKeys(key);

            //Каждый блок по отдельности
            foreach (var Block8 in bytes)
            {
                //2. Начальная перестановка битов (IP)
                BitArray Bits64 = new(Block8);

                // Изменение порядка битов в каждом блоке по 8 бит
                for (int i = 0; i < Bits64.Length; i += 8)
                {
                    bool[] reversedBlock = new bool[8];
                    for (int j = 0; j < 8; j++)
                    {
                        reversedBlock[j] = Bits64[i + (7 - j)]; // Переворачиваем порядок битов
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        Bits64[i + j] = reversedBlock[j]; // Записываем перевернутый блок обратно
                    }
                }

                Bits64 = Swap(Bits64, IP);

                BitArray L = new(32);
                BitArray R = new(32);
                CopyBaToLR(Bits64, L, R); //Возможно тут другой порядок

                //3. Циклы преобразований
                for (int i = 15; i >= 0; i--)
                {
                    var oldR = R;
                    R = L.Xor (Func(oldR, Keys[i]));
                    L = oldR;
                }

                BitArray resBits = new BitArray(64);
                for (int i = 0; i < 32; i++)
                {
                    resBits[i] = L[i];
                    resBits[32 + i] = R[i];
                }

                //Обратная перестановка (IP_INV)
                resBits = Swap(resBits, IP_INV);

                resBits.CopyTo(byteArray, bytePosition);
                bytePosition += 8;
            }

            return Encoding.ASCII.GetString(byteArray);
        }

        /// <summary>
        /// 32-битный полублок
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static BitArray Func(BitArray bits, BitArray key)
        {
            BitArray extBits = new BitArray(48);
            //Вроде правильно
            for(int i = 0;i < E.Length; i++)
            {
                int bitIndex = E[i]-1;
                extBits[i] = bits[bitIndex];
                //Console.WriteLine($"{i+1} -> {bitIndex}");
            }
            //Гаммируем с ключем
            extBits.Xor(key);

            // Разделение на S блоки
            List<BitArray> SBlocks = new List<BitArray>();
            for (int i = 0; i < 48; i += 6)
            {
                BitArray sBlock = new BitArray(6);
                for (int j = 0; j < 6; j++)
                    sBlock[j] = extBits[i + j];

                SBlocks.Add(sBlock);
            }

            //Работа с S блоками (правильно 100%)
            BitArray resBits = SFunc(SBlocks);

            //Перестановка P
            resBits=Swap(resBits, P);
            return resBits;
        }

        private static BitArray SFunc(List<BitArray> SBlocks)
        {
            BitArray resFunc = new BitArray(32);
            int bitPos = 0;  // Позиция для записи результата в resFunc

            for (int i = 0; i < SBlocks.Count; i++)
            {
                BitArray block = SBlocks[i];
                // Получаем строку из первого и шестого битов
                int row = (block[0] ? 1 : 0) << 1 | (block[5] ? 1 : 0);
                // Получаем столбец из битов 2, 3, 4 и 5
                int col = (block[1] ? 1 : 0) << 3 | (block[2] ? 1 : 0) << 2 | (block[3] ? 1 : 0) << 1 | (block[4] ? 1 : 0);

                int sValue = S_BOXES[i, row, col];

                // Из 10-ого числа в 2-значное 4 бита (т.к. макс 15 в таблице)
                for (int j = 3; j >= 0; j--)
                    resFunc[bitPos++] = (sValue & (1 << j)) != 0;
                //Вроде как правильно

            }
            return resFunc;
        }

        /// <summary>
        /// Функция перестановки
        /// </summary>
        /// <param name="bitArr"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        private static BitArray Swap(BitArray bitArr, int[] table)
        {
            BitArray swapBlock = new BitArray(table.Length);
            for (int i = 0; i < table.Length; i++)
            {
                int bitIndex = table[i] - 1;
                swapBlock[i] = bitArr[bitIndex];
            }

            return swapBlock;
        }

        private static List<byte[]> To8Byte(string text) //МБ тут надо 
        {
            List<byte[]> res = new List<byte[]>();
            byte[] bytes = Encoding.UTF8.GetBytes(text);


            for (int i = 0; i < bytes.Length; i += 8)
            {
                byte[] block = new byte[8];
                int bytesToCopy = Math.Min(8, bytes.Length - i);

                Array.Copy(bytes, i, block, 0, bytesToCopy);

                //Если последний блок <8байт то заполняем
                if (bytesToCopy < 8)
                {
                    for (int j = bytesToCopy; j < 8; j++)
                        block[j] = 0;
                }

                res.Add(block);
            }

            return res;
        }

        private static List<byte[]> To8Byte2(byte[] text) //МБ тут надо 
        {
            List<byte[]> res = new List<byte[]>();

            for (int i = 0; i < text.Length; i += 8)
            {
                byte[] block = new byte[8];
                int bytesToCopy = Math.Min(8, text.Length - i);

                Array.Copy(text, i, block, 0, bytesToCopy);

                //Если последний блок <8байт то заполняем
                if (bytesToCopy < 8)
                {
                    for (int j = bytesToCopy; j < 8; j++)
                        block[j] = 0;
                }

                res.Add(block);
            }

            return res;
        }

        private static List<BitArray> CreateKeys(long key)
        {
            List<BitArray> result = new List<BitArray>();
            BitArray res = new BitArray(56);
            BitArray keyBits = new BitArray(BitConverter.GetBytes(key));// Правильно

            BitArray reversedBits = new BitArray(64);
            // Переворачивание битов
            for (int i = 0; i < keyBits.Length; i++)
            {
                reversedBits[keyBits.Length - 1 - i] = keyBits[i];
            }

            // Копирование перевернутого массива обратно в res
            for (int i = 0; i < keyBits.Length; i++)
            {
                keyBits[i] = reversedBits[i];
            }


            //перестановка PC1
            for (int i = 0; i < PC1.Length; i++)
            {
                int bitIndex = PC1[i] - 1;
                res[i] = keyBits[bitIndex];
            }

            BitArray LKey = new(28);
            BitArray RKey = new(28);
            CopyBaToLR(res, LKey, RKey);

            //Шестандцать - 56 битных ключей
            for (int i=0; i < 16; i++)
            {
                //Сдвиг влево
                Shift(LKey, i);
                Shift(RKey, i);

                BitArray Key56 = new BitArray(56);

                for (int k = 0; k < 28; k++)
                {
                    Key56[k] = LKey[k];
                    Key56[28 + k] = RKey[k];
                }

                BitArray resKey48 = new BitArray(48);
                //перестановка PC2
                for (int j = 0; j < 48; j++)
                {
                    int bitIndex = PC2[j] - 1;
                    resKey48[j] = Key56[bitIndex];
                    //Console.WriteLine($"Бит:{j} -> {PC1[j] - 1}");
                }

                result.Add(resKey48);
            }

            return result;
        } 

        /// <summary>
        /// Копирование половинок <3
        /// </summary>
        /// <param name="source"> Откуда </param>
        /// <param name="lArray">Левая (первая половина)</param>
        /// <param name="rArray">Правая (вторая половина)</param>
        private static void CopyBaToLR(BitArray source, BitArray lArray, BitArray rArray)
        {
            int halfLength = source.Length / 2;

            for (int i = 0; i < halfLength; i++)
            {
                lArray[i] = source[i];
                rArray[i] = source[i + halfLength];
            }
        }

        /// <summary>
        /// Сдвиг по таблице SHIFT_Key
        /// </summary>
        /// <param name="bits">Массив бит, которые надо сдвинуть</param>
        /// <param name="indexKey">Индекс в таблице SHIFT_Key</param>
        private static void Shift(BitArray bits, int indexKey)
        {
            int shiftNum = SHIFT_Key[indexKey];
            int length = bits.Length;

            bool[] shiftedBits = new bool[length];

            for (int i = 0; i < length; i++)
            {
                int newIndex = (i + length - shiftNum) % length; // Находим новый индекс с учетом циклического сдвига
                shiftedBits[newIndex] = bits[i];
            }

            for (int i = 0; i < length; i++)
            {
                bits[i] = shiftedBits[i];
            }
        }
    }
}