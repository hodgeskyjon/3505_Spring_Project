// Representation of cells for the spreadsheet

#include <string>
#include <map>
#include <iostream>
#include <iterator>
#include <vector>
#include "dependency_graph.h"
#include <set>
#include <list>

namespace cs3505
{

    dependency_graph::dependency_graph()
	{
	}    // Constructor
    
	dependency_graph::~dependency_graph() 
	{
	}	// Destructor
	
	// returns the size of the dependency graph
	int dependency_graph::size()			
	{
		return set_of_dependents.size();
	}

	// returns the number of dependees of a specific cell
	int dependency_graph::num_dependees(std::string cell)
	{
		int count = 0;
		std::multimap<std::string, std::string>::iterator it;

		for(it=set_of_dependents.begin(); it !=set_of_dependents.end(); ++it)
		{
			if(it->first == cell)
				count++;
		}
		
		return count;
	}   

    // returns true if the cell has dependents
	bool dependency_graph::has_dependents(std::string cell)
	{
		if(set_of_dependents.find(cell) != set_of_dependents.end())
			return true;
		else
			return false;
	}

	// returns true if the cell has dependees
	bool dependency_graph::has_dependees(std::string cell)
	{
		std::multimap<std::string, std::string>::iterator it;
		for (it=set_of_dependents.begin(); it !=set_of_dependents.end(); ++it)
		{
			if(it->second == cell)
				return true;
		}
		
		return false;
	}	

	// returns a vector of all the cells dependent upon this cell
	std::vector<std::string> dependency_graph::get_dependents(std::string cell)
	{
		std::vector<std::string> list_of_dependents;
		std::multimap<std::string, std::string>::iterator it;
		
		for(it=set_of_dependents.begin(); it !=set_of_dependents.end(); ++it)
		{
			if(it->first == cell)
				list_of_dependents.push_back(it->second);
		}
		
		return list_of_dependents;
	}

	// adds a dependency to the graph
	void dependency_graph::add_dependency(std::string cell_a, std::string cell_b)  
	{
		std::multimap<std::string, std::string>::iterator it;
		bool in_graph = false;
		
		for (it=set_of_dependents.begin(); it !=set_of_dependents.end(); ++it)
		{
			if(it->first == cell_a && it->second == cell_b)
				in_graph = true;
		}
		
		if(!in_graph)
			set_of_dependents.insert(std::make_pair(cell_a, cell_b));
	}
	
	// removes a dependency from the graph
	void dependency_graph::remove_dependency(std::string cell_a, std::string cell_b)  
	{
		std::multimap<std::string, std::string>::iterator it;
		bool in_graph = false;
		
		for (it=set_of_dependents.begin(); it !=set_of_dependents.end(); ++it)
		{
			if(it->first == cell_a && it->second == cell_b)
			{
				in_graph = true;
				break;
			}
		}
		
		if(in_graph)
			set_of_dependents.erase(it);
	}
	
	// replaces dependents
	void dependency_graph::replace_dependents(std::string cell, std::vector<std::string> list) 
	{
		std::multimap<std::string, std::string>::iterator it;
		int num_dependents = this->num_dependees(cell);
		
		for(int i = 0; i < num_dependents; ++i)
		{
			it = set_of_dependents.find(cell);
			set_of_dependents.erase(it);
		}
		
		for(int i=0; i<list.size(); i++)
		{
			add_dependency(cell, list[i]);
		}
	}	

	// replaces dependees
	void dependency_graph::replace_dependees(std::string cell, std::vector<std::string> list)
	{
		std::multimap<std::string, std::string>::iterator it;
		std::vector<std::string>::iterator it_list;
		
		std::vector<std::string> dependee_list;
		
		for(it = set_of_dependents.begin(); it!=set_of_dependents.end(); ++it)
		{	
		  if(it->second == cell)
			  dependee_list.push_back(it->first);
		}
		
		for(int i=0; i<dependee_list.size(); i++)
		{
		  remove_dependency(dependee_list[i], cell);
		}
		
		for(int i=0; i<list.size(); i++)
		{
		  add_dependency(list[i], cell);
		}
	}


  //ported 
  std::list<std::string> dependency_graph::get_cells_to_recalc(std::string name)
  {
    std::list<std::string> changed;
    std::set<std::string> visited;
    std::set<std::string> names;
    std::set<std::string>::iterator it;
    std::list<std::string>::iterator i;
    
    names.insert(name);
    
    for (it = names.begin(); it != names.end(); ++it)
      {
	if (visited.find(*it) != visited.end() == false)
	  {
	    visit(*it, *it, visited, changed);
	  }
      } 
    
    return changed;
  }
  
  //helper function for get_cells_to_recalc
  void dependency_graph::visit(std::string start, std::string name, std::set<std::string> visited, std::list<std::string> changed)
  {
    visited.insert(name);
    std::set<std::string>::iterator it;
    
    for (int i = 0; i < get_dependents(name).size(); i++)
      {
	if (get_dependents(name).at(i) == start)
	  {
	    std::cout << "***CIRCULAR DEPENDENCY***" << std::endl; //placeholder for error message or other action
	  }
	
	else if (visited.find(get_dependents(name).at(i)) != visited.end() == false)
	  {
	    visit(start, get_dependents(name).at(i), visited, changed);
	  }
      }
    changed.push_front(name);
  }

}
