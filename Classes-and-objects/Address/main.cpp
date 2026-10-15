#include <Address.hpp>
#include <iostream>

int main()
{
    Address address("123 Main Street", "Boston", "MA", "02108");

    std::cout << address.ToString() << std::endl;
}