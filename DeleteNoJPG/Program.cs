using System;
using System.IO;


namespace DeleteNoJPG
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //사용자 변경////////////////////////////////////////////////////////////
            String dirName = "D:\\ImgBackup\\201702";//비교할 대상이 있는 디렉토리 경로. 하위 디렉토리까지 전부 검색한다.

            const string jpgHeader = "C:/DEV/DeleteNoJPG/JPGHeader.lhw"; //jpg 특유의 헤더를 바이너리화한 파일의 경로.
            String searchType = "*.jpg";//찾아낼 대상의 확장자. *.*은 모든 파일에서 jpg 형식이 아닌걸 삭제한다.
            //사용자 변경////////////////////////////////////////////////////////////

            clearDir(dirName, jpgHeader, searchType);
            Console.WriteLine("완료");

        }

        private static void clearDir(String dirName, String jpgHeader, String searchType)
            //하위 디렉토리를 검색후 삭제하는 함수
        {
            byte[] jpgHeaderByte = File.ReadAllBytes(jpgHeader);//파일 헤더의 바이너리를 바이트 스트림으로 추출.
            byte[] searchedFileHeaderByte;//디렉토리 속 파일의 바이너리를 읽을 바이트 배열

            if (Directory.Exists(dirName))
                //해당 디렉토리가 있으면 실행함.
                //재귀함수 호출의 끝을 알리는 비교문.
            {
                DirectoryInfo dirnames = new DirectoryInfo(dirName);//디렉토리 안의 정보를 가진 객체

                foreach (DirectoryInfo item in dirnames.GetDirectories())
                {
                    Console.WriteLine("재귀 호출");
                    clearDir(item.FullName, jpgHeader, searchType);//재귀적 호출 가장 하위 디렉토리까지 탐색(디렉토리들이 스택으로 쌓임 = 하위부터 처리.)

                    //재귀 함수 호출 스택에서 실행될 부분.
                    DirectoryInfo di = new DirectoryInfo(item.FullName);//하위 디렉토리 파악.
                    FileInfo[] subfiles = di.GetFiles(searchType);

                    //파일 적합성 검사.
                    Console.WriteLine("\n파일 적합성 검사");

                    //jpg 파일 클리어
                    if (subfiles.Length != 0)
                    {
                        foreach (FileInfo fi in subfiles)//검색한 파일들의 풀네임을 하나씩 대입시킴.
                        {
                            bool isjpg = true;
                            searchedFileHeaderByte = File.ReadAllBytes(fi.FullName);//찾은 파일의 바이너리를 바이트 스트림으로 추출

                            if (jpgHeaderByte.Length>=searchedFileHeaderByte.Length)
                                //샘플인 헤더 바이너리 크기가 비교 파일보다 더 길시 해당 파일은 비정상 파일이므로...
                                //저절로 길이가 0인 파일도 제거가 됨.
                            {
                                isjpg = false;
                            }
                            else
                            {
                                for (int i = 0; i < jpgHeaderByte.Length; i++)//비교 파일 헤더 바이너리만큼 비교
                                {
                                    if (jpgHeaderByte[i] != searchedFileHeaderByte[i])//만약 바이너리가 일치하지 않으면
                                    {
                                        isjpg = false;
                                        break;
                                    }
                                    //만약 바이너리가 일치하면
                                    isjpg = true;

                                }
                            }
                            

                            //한 파일 비교 완료.
                            if (isjpg)
                            {
                                Console.WriteLine(fi.FullName + "는 jpg입니다.");
                            }
                            else
                            {
                                Console.Write(fi.FullName + "는 jpg가 아닙니다.");
                                fi.Delete();
                                Console.WriteLine(" (Delete)");
                            }

                        }
                    }


                    //빈 디렉토리 클리어
                    Console.WriteLine("디렉토리 검사");
                    if (di.GetFiles("*.*").Length == 0 && di.GetDirectories().Length ==0)
                    {
                        Console.Write(item.FullName+"는 비어있습니다.");
                        di.Delete();
                        Console.WriteLine(" (Delete)");
                    }
                    else
                    {
                        Console.WriteLine(item.FullName + "는 비어있지 않습니다.");
                    }

                }

            }
        }

    }
}
