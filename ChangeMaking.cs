
using System;
using System.Collections.Generic;

namespace ASD
{

    class ChangeMaking
    {

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// bez ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to metochange = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości amount ( czyli rzędu o(amount) )
        /// </remarks>
        public static int? NoLimitsDynamic(int amount,int[] coins,out int[] change)
        {
            int?[,] best = new int?[amount + 1,2];
            best[0,0] = 0;
            change = new int[coins.Length];

            //Dla zestawów monet: {0}, {0,1}, {0,1,2}, ..., {0,1,...,coins.Length-1}
            for (int q = 0; q < coins.Length; q++)
            {
                //Oblicz rozwiązanie dla kwot od 1 do amount
                for (int i = 1; i <= amount; i++)
                {
                    //Dla każdej monety z zestawu
                    for (int j = 0; j <= q; j++)
                    {
                        //Jeśli aktualny nominał jest za duży
                        if (coins[j] > i)
                            continue;
                        //Jeśli istnieje rozw. dla kwoty pomniejszonej o nominał ORAZ będzie ono lepsze od aktualnego-1
                        if (best[i - coins[j],0] != null && (best[i,0] == null || 1 + best[i - coins[j],0] < best[i,0]))
                        {
                            best[i,0] = 1 + best[i - coins[j],0];
                            best[i,1] = j;
                        }
                    }
                }
            }

            if (coins.Length == 0 || best[amount,0] == null)
            {
                change = null;
                return null;
            }
            else
            {
                int k = amount;
                while (k > 0)
                {
                    int id = (int)best[k,1];

                    k -= coins[(int)best[k,1]];
                    change[id]++;
                }

                return best[amount,0];
            }


        }

        /// <summary>
        /// Metoda wyznacza rozwiązanie problemu wydawania reszty przy pomocy minimalnej liczby monet
        /// z uwzględnieniem ograniczeń na liczbę monet danego rodzaju
        /// </summary>
        /// <param name="amount">Kwota reszty do wydania</param>
        /// <param name="coins">Dostępne nominały monet</param>
        /// <param name="limits">Liczba dostępnych monet danego nomimału</param>
        /// <param name="change">Liczby monet danego nominału użytych przy wydawaniu reszty</param>
        /// <returns>Minimalna liczba monet potrzebnych do wydania reszty</returns>
        /// <remarks>
        /// coins[i]  - nominał monety i-tego rodzaju
        /// limits[i] - dostepna liczba monet i-tego rodzaju (nominału)
        /// change[i] - liczba monet i-tego rodzaju (nominału) użyta w rozwiązaniu
        /// Jeśli dostepnymi monetami nie da się wydać danej kwoty to change = null,
        /// a metoda również zwraca null
        ///
        /// Wskazówka/wymaganie:
        /// Dodatkowa uzyta pamięć powinna (musi) być proporcjonalna do wartości iloczynu amount*(liczba rodzajów monet)
        /// ( czyli rzędu o(amount*(liczba rodzajów monet)) )
        /// </remarks>
        public static int? Dynamic(int amount,int[] coins,int[] limits,out int[] change)
        {
            int?[,] best = new int?[coins.Length + 1,amount + 1];
            int?[,] lastCoin = new int?[coins.Length + 1,amount + 1];
            for (int i = 0; i <= coins.Length; i++)
                best[i,0] = 0;
            for (int i = 1; i <= amount; i++)
                best[0,i] = null;
            int[,] limits2d = new int[limits.Length,amount + 1];
            for (int i = 0; i < amount + 1; i++)
                for (int j = 0; j < limits.Length; j++)
                    limits2d[j,i] = limits[j];
            change = new int[coins.Length];

            
            //Dla zestawów monet: {0}, {0,1}, {0,1,2}, ..., {0,1,...,coins.Length-1}
            for (int j = 1; j <=  coins.Length; j++)
            {
                //Oblicz rozwiązanie dla kwot od 1 do amount
                for (int i = 1; i <= amount; i++)
                {
                    best[j,i] = best[j - 1,i];
                    lastCoin[j,i] = lastCoin[j - 1,i];
                    
                    //Dla każdej monety z zestawu
                    for (int q = 0; q < j; q++)
                    {
                        //Jeśli:      1.  moneta nie jest większa niż rozpatrywana kwota
                        //       ORAZ 2.  ta moneta jest dostępna
                        //       ORAZ 3.  znamy rozwiązanie dla kwoty pomniejszonej o tę monetę
                        //       ORAZ 4.  nowe rozwiązanie jest lepsze            
                        if (coins[q] <= i &&
                            limits2d[q,i - coins[q]] != 0 &&
                            best[j,i - coins[q]] != null &&
                            (best[j,i] == null || 1 + best[j,i - coins[q]] < best[j,i]))//best[j - 1,i] == null || 1 + best[j,i - coins[q]] < best[j - 1,i]))
                        {
                            best[j,i] = 1 + best[j,i - coins[q]];
                            lastCoin[j,i] = q;
                            for (int x = 0; x < limits.Length; x++)
                                limits2d[x,i] = limits2d[x,i - coins[q]];
                            limits2d[q,i]--;
                        }


                    }


                }
            }

            if (coins.Length == 0 || best[coins.Length,amount] == null)
            {
                change = null;
                return null;
            }
            else
            {
                int k = amount;
                for (int i = 0; i < coins.Length; i++)
                    change[i] = limits[i] - limits2d[i,amount];

                return best[coins.Length,amount];
            }


        }

    }

}
