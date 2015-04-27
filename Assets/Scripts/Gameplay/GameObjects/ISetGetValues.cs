using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public interface ISetGetValues
{
    /**
	 * Sets default variables for the controller from information provided in
	 * a json file
	 * 
	 * @param values
	 * 		A json fiendly dictionary with values for the controller
	 */
    void SetValues(IDictionary values);

    /**
     * Gets all the values in the controller and returns them in a json frinedly 
     * dictionary.
     * 
     * @return 
     *      A json friendly dictionary.
     */
    IDictionary GetValues();
}

