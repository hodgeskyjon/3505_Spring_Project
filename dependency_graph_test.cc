
#include <iostream>
#include <fstream>
#include <map>
#include <set>
#include <iterator>
#include <sstream>
#include <vector>
#include "dependency_graph.h"

using namespace std;

/*
 * Test for dependency graph.
 */
int main (int argc, char *argv[])
{
	cs3505::dependency_graph *t = new cs3505::dependency_graph();
	
	t->add_dependency("a", "b");
	t->add_dependency("a", "c");
	t->add_dependency("b", "a");
	t->add_dependency("d", "b");
	
	cout << "graph size: " << t->size() << endl;
	cout << "a dependees: " << t->num_dependees("a") << endl;
	cout << "a has dependents: " << t->has_dependents("a") << endl;
	cout << "a has dependees: " << t->has_dependees("a") << endl;
	
	std::vector<std::string> dependents = t->get_dependents("a");
	
	for(int i = 0; i < dependents.size(); i++)
	{
		cout << "dependent of a: " << dependents[i] << endl;
	}
	
	std::vector<std::string> new_dependees;
	new_dependees.push_back("e");
	new_dependees.push_back("f");
	new_dependees.push_back("g");
	new_dependees.push_back("h");
	new_dependees.push_back("i");
	new_dependees.push_back("j");

	t->replace_dependents("a", new_dependees);
	
	dependents = t->get_dependents("a");
	
	for(int i = 0; i < dependents.size(); i++)
	{
		cout << "dependent of a: " << dependents[i] << endl;
	}
	
	t->replace_dependees("a", new_dependees);
	
	dependents = t->get_dependents("e");
	
	for(int i = 0; i < dependents.size(); i++)
	{
		cout << "dependent of e: " << dependents[i] << endl;
	}
	
	t->remove_dependency("a", "b");
	
	cout << "graph size: " << t->size() << endl;
	
	
	return 0;
}
	