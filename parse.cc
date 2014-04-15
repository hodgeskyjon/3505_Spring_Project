#include <boost/algorithm/string.hpp>
#include <iostream>
#include <string>
#include <vector>

bool is_var(std::string s)
{
  if (isalpha(s[0])) return true;
}

int main()
{

  std::string s = "(a5 +b77 - 7 + (6-13)-5)";
  std::vector <std::string> fields;

  boost::split(fields, s, boost::is_any_of( " ()+-=/*," ) );

 for (int i = 0; i < fields.size(); i++)
   {
     if (is_var(fields[i]))
       {
	 std::cout << fields[i] << std::endl;
       }
   }

  return 0;
}


