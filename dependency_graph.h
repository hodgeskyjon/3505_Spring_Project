/* The warehouse object header class
 *
 * Skyler Jones & Christy Bowen
 * January 26, 2014
 */

#ifndef DEPENDENCY_GRAPH_H
#define DEPENDENCY_GRAPH_H
#include <vector>
#include <string>
#include <map>
#include <set>
#include <list>

namespace cs3505
{
    /*
     * This class will represent the model of a
     * spreadsheet
     */
    class dependency_graph
    {
    public:
        
        dependency_graph();    // Constructor
        ~dependency_graph();   // Destructor
        
	int size();			// returns the size of the dependency graph
	int num_dependees(std::string);   // returns the number of dependees of a specific cell
	bool has_dependents(std::string);  // returns true if the cell has dependents
	bool has_dependees(std::string);   // returns true if the cell has dependees
	std::vector<std::string> get_dependents(std::string);  // returns a vector of all the cells dependent upon this cell
	void add_dependency(std::string, std::string);  // adds a dependency to the graph
	void remove_dependency(std::string, std::string);  // removes a dependency from the graph
	void replace_dependents(std::string, std::vector<std::string>);  // replaces dependents
        void replace_dependees(std::string, std::vector<std::string>);  // replaces dependees
	std::list<std::string> get_cells_to_recalc(std::string);
		
	private:
		
	std::multimap<std::string, std::string> set_of_dependents; // holds the set of dependent/dependees
	void visit(std::string start, std::string name, std::set<std::string> visited, std::list<std::string> changed);
	
	
    };
    
}
#endif
