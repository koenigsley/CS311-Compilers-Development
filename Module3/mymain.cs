using System;
using System.IO;
using SimpleScanner;
using ScannerHelper;

namespace GeneratedLexer
{

    class mymain
    {
        static void Main(string[] args)
        {
            int cnt_id = 0;//���-�� ���������������
            int min_id_len = int.MaxValue, max_id_len = 0; //�����������, ������������ ����� ���������������
            double avg_id_len = 0; //������� ����� ��������������

            int sum_int = 0; //����� ���� �����
            double sum_d = 0; //����� ���� ������������

            // ����� ������������ ����� �������������� � ������������ � ������� 3.14 (� �� 3,14 ��� � ������� Culture)
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            var fname = @"..\..\a.txt";
            Console.WriteLine(File.ReadAllText(fname));
            Console.WriteLine("-------------------------");

            var scanner = new Scanner(new FileStream(fname, FileMode.Open));

            int tok;
            do {
                tok = scanner.yylex();

                if (tok == (int)Tok.ID)
                {
                    cnt_id += 1;
                    int idLen = scanner.yytext.Length;
                    avg_id_len += idLen;
                    min_id_len = Math.Min(min_id_len, idLen);
                    max_id_len = Math.Max(max_id_len, idLen);
                }
                else if (tok == (int)Tok.INUM)
                {
                    sum_int += scanner.LexValueInt;
                }
                else if (tok == (int)Tok.RNUM)
                {
                    sum_d += scanner.LexValueDouble;
                }
                else if (tok == (int)Tok.EOF)
                {
                    avg_id_len /= cnt_id;

                    Console.WriteLine();
                    Console.WriteLine("number of id: {0:D}", cnt_id);
                    Console.WriteLine("average length of the id: {0:N}", avg_id_len / cnt_id);
                    Console.WriteLine("min length of the id: {0:D}", min_id_len);
                    Console.WriteLine("min length of the id: {0:D}", max_id_len);

                    Console.WriteLine();
                    Console.WriteLine("sum of int: {0:D}", sum_int);
                    Console.WriteLine("sum of double: {0:N}", sum_d);

                    Console.WriteLine();

                    break;
                }

                Console.WriteLine(scanner.TokToString((Tok)tok));
            } while (true);

            Console.ReadKey();
        }
    }
}
