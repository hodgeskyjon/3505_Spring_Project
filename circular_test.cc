
#include <iostream>
#include <fstream>
#include <map>
#include <set>
#include <iterator>
#include <sstream>
#include <vector>
#include "dependency_graph.h"

using namespace std;

int main (int argc, char *argv[])
{
	cs3505::dependency_graph *t = new cs3505::dependency_graph();
	
	t->add_dependency("a", "b");
	t->add_dependency("a", "c");
	t->add_dependency("c", "d");

	//*******Test 1: No dependency************

	std::cout << "Test 1: Should print nothing:" << std::endl;
	t->get_cells_to_recalc("a");

	//********Test 2: Add dependency to itself**********

	t->add_dependency("a", "a");
	std::cout << "Test 2: Circular dependency to self:" << std::endl;
	t->get_cells_to_recalc("a");

	//******Test 3: Remove self-dependency and check indirect dependency

	t->remove_dependency("a", "a");
	t->add_dependency("d", "a");
	std::cout << "Test 3: Indirect Circular Dependency:"<< std::endl;
	t->get_cells_to_recalc("a");
	
	return 0;
}
	
