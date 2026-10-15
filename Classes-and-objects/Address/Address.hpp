#ifndef ADDRESS_HPP
#define ADDRESS_HPP

#include <string>
#include <sstream>
#include <stdexcept>


class Address
{
    private:
    const std::string street;
    const std::string city;
    const std::string state;
    const std::string zip;

    public:
    Address(const std::string street,
    const std::string city,
    const std::string state,
    const std::string zip);
    std::string ToString() const;
};

#endif