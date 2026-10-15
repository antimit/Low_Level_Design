#include <string>
#include <vector>
#include <algorithm>
#include <numeric>
#include <iostream>

using namespace std;
class Student
{
    private:
    string _Name;
    string _ID;
    vector<int> grades;
    
    
    public:
    Student(string Name, string ID) : _Name(Name), _ID(ID){}
    int calculate_average_grade()
    {
        return accumulate(grades.begin(), grades.end(), 0) / ((grades.size() > 0) ? grades.size() : 1);
    }

    void add_new_grade(int grade)
    {
        grades.push_back(grade);
    }
};

int main ()
{
    Student student("Francis", "AT54");
    student.add_new_grade(5);
    student.add_new_grade(10);
    student.add_new_grade(20);
    student.add_new_grade(30);
    student.add_new_grade(40);
    student.add_new_grade(50);

    int res = student.calculate_average_grade();

    std::cout <<"The average grade is : "<< res<<std::endl;
}