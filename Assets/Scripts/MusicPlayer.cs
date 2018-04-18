using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {
	
/*great example of singleton
we call a static instance of this class
static means every instance shares the same property of this class
where when the property of this static changes, every instance
from this class will change along with it.

in this case, we have a static instance, where this class only has
one instance and every object of this class shares this one instance.
it is initialized as null, meaning there are no instances of this class. 
then if the instance isn't null we will destroy the created instance of
this object, or the duplicate object gets destroyed in other words.

else we create this instance this. it is called in the awake method 
so that we destroy the duplicate before it hits the start, where you
get a blimp of music playing before the duplicate gets destroyed
*/
		
	static MusicPlayer instance = null;

	void Awake () {	
		if (instance != null){
			Destroy(gameObject);
		}else{ 
			instance = this;
			GameObject.DontDestroyOnLoad(gameObject);
		}
	}
}
