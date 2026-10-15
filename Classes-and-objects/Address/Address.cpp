#include <Address.hpp>


std::string Address::ToString() const
{
    std::ostringstream oss;

    oss << this->street + ","
    <<this->city  + ","
    << this->state + ","
    << this->zip + ",";

    return oss.str();
}


Address::Address(const std::string street,
    const std::string city,
    const std::string state,
    const std::string zip) : street(street), city(city), state(state), zip(zip)
{
    if(street.empty())
    {
        throw std::invalid_argument("Street cannot be empty");
    }

    if(city.empty())
    {
        throw std::invalid_argument("City cannot be empty");
    }
}