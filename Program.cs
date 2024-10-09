// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

Console.WriteLine("Hello, World!");
ChessBoard board = new();
Random rand = new Random();
unsafe
{
       ChessBoard* k = (ChessBoard*) &board;

        GCHandle handle = GCHandle.Alloc(board);
        k->PopulateArr();
        k->DisplayValues();
        Console.Read();
        handle.Free();
        GC.Collect();

}

public class ChessBoard
{
    public ulong[,] Board { get; set; } =  new ulong[8, 8];

    public unsafe void DisplayValues()
    {
        ulong pop = 0;
        ulong goog = 0;
        bool displaygoog = true, displaypop = true;
        LoopOver(null,
           new Action<int?, object[]?>[] { (_, _) => { Console.WriteLine(); Console.WriteLine(); } },
           new PerItemCallBack[]
           {
                (in int row, in int iter,in ulong* item, object[]? items)=>{
                    Console.Write("{0} ", *item);
                    pop = (*item >= 10000000 && pop == 0) ? *item : pop;
                    if(pop > 0 && displaypop){
                        Console.WriteLine("[k,i]: [{0},{1} ::: val {2}]", row, iter, *item);
                        displaypop = false;
                    }
                    goog = (*item >= 1340000000000 && goog == 0) ? *item : goog;
                    if(goog > 0 && displaygoog){
                        Console.WriteLine("[k,i]: [{0},{1} ::: val {2}]", row, iter, *item);
                       displaygoog =false; 
                    }
                },
           }, null, null, null);
        
    }

    public unsafe delegate void PerItemCallBack(in int row, in int iterAtRow,in ulong* item, params object[]? objects);

    public unsafe void PopulateArr()
    {
        ulong a = 1;
        LoopOver(null,
           new Action<int?, object[]?>[] { (_, _) => { Console.WriteLine(); } },
           new PerItemCallBack[]
           {
                (in int row, in int iter,in ulong* item, object[]? items)=>{
                    if(row == 0 && iter == 0) *item = 1;
                    else *item = (a = a * 2);
                }
           }, null, null, null);
       
   }
    private unsafe void LoopOver(Action<int?, object[]?>[]? PerRowBefore, Action<int?, object[]?>[]? PerRowAfter, PerItemCallBack[] callback,
        object[]? before,
        object[]? after,
        object[]? perItem)
    {
        unsafe
        {
            for (int i = 0; i < (Board.Length / Board.GetLength(0)); i++)
            {
                if(PerRowBefore is not null) foreach (var func in PerRowBefore) func(i, before);
                for (int k = 0; k < Board.GetLength(0); k++)
                {
                    fixed(ulong* a = &Board[i, k])
                    {
                        foreach (var func in callback) func(i, k, in a, perItem);
                    }
                }
                if(PerRowAfter is not null) foreach (var func in PerRowAfter) func(i, after);
            }
        }
    }
}
