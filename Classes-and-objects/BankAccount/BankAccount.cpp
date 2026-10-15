
#include <iostream>
#include <string>
#include <vector>
#include <sstream>

class BankAccount
{
    public:
    BankAccount() {
        this->currentSum = 0;
    }
    void deposit(int sum)
    {
        this->currentSum += sum;

        std::string transaction = "New deposit was conducted :" + std::to_string(sum) + ", current balance is: " + std::to_string(this->currentSum);
        this->transaction_history.push_back(transaction);
    }

    int withdraw(int sum)
    {
        int withdrawal = -1;
        if(sum <= this->currentSum)
        {
            this->currentSum -= sum;
            withdrawal = sum;
        }   
        
        std::string transaction = "New withdrawal was conducted :" + std::to_string(withdrawal) + ", current balance is: " + std::to_string(this->currentSum);
        this->transaction_history.push_back(transaction);
        return withdrawal;
    }

    std::string get_transaction_history()
    {
        std::ostringstream oss;

        for(std::string & transaction : transaction_history)
        {
            oss << transaction << std::endl;
        }

        return oss.str();
    }
    private: 
    double currentSum;
    std::vector<std::string> transaction_history;
}; 
int main()
{
    BankAccount b;
    std::cout << "Depositing 100" << std::endl;
    b.deposit(100);

    
    int withdrawalTest1 = b.withdraw(100);
    std::cout << "Withdrawing 100 : " << withdrawalTest1 << std::endl;
    
    int withdrawalTest2 = b.withdraw(100);
    std::cout << "Withdrawing 100 again " << withdrawalTest2<<  std::endl;

    std::string history = b.get_transaction_history();

    std::cout <<history << std::endl;
}