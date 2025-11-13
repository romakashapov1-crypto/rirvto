    using System;
    using System.Threading;

    public class BankAccount
    {
        private decimal balance;
        private readonly Mutex mutex = new Mutex();

        public void Deposit(decimal amount)
        {
            mutex.WaitOne();
            try
            {
                balance += amount;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public void Withdraw(decimal amount)
        {
            mutex.WaitOne();
            try
            {
                if (balance >= amount)
                {
                    balance -= amount;
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        public decimal GetBalance()
        {
            mutex.WaitOne();
            try
            {
                return balance;
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var account = new BankAccount();
            var random = new Random();
            var threads = new Thread[10];

            for (int i = 0; i < 10; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        if (random.Next(2) == 0)
                            account.Deposit(10);
                        else
                            account.Withdraw(10);
                    }
                });
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine($"Итоговый баланс: {account.GetBalance()}");
        }
    }