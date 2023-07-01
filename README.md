
## To-dos
- [x] Capture Mode
	- [x] Store the **Position** and **Rotation** of captured images in capture mode.
	- [ ] Add placing cude in capture mode
		- [x] Plane detection
		- [x] Place cude
		- [x] Store the Position and Rotation of the cube in json file
		- [ ] !!! To be fixed: The position of the cube is extremely large, don't know where lies the problem. So I choose to use a cube in fixed position (1,0,0).
- [x] Detection Mode
	- [x] Get the Position and Rotation of a detected image
	- [x] Calculate the offset between the previous and current coordinate systems.
		- [x] While detecting the first image
		- [x] While detecting the second image and so on
	- [x] Calculate and show the cube in current system.
    
---

## Gitflow

Our development will follow the workflow "Gitlow"

We will have **a master branch, a develop branch, several hotfixes, release, feature branches**

1. Only hotfixes and release branches can merge to the master branch.
2. hotfixes only fix bugs from master
3. develop must first merge to release and fix bugs there
4. feature branches will merge to develop branch

![img](https://lh5.googleusercontent.com/lQi1fsL5G88MPd9kZRjXGGDb-pUqQW0aVIA4VrP9cgfjuy8j7a7cnQ1_7nW5dZrH0-QJCA-SOkKDq8utCZwvrY8KwHOvZWUsj44oSLP3AZ_sLSutTTWwNyp6WSPRNcUGd23r95kIz8xLNyaa7TdpA5xzBq2fRMPI_HCdwsGYHhYvyzb7T70a9GjoLRcsyw)

---
## Presentation

Here is a link to the presentation slides of this project: https://docs.google.com/presentation/d/1kFmRBdnlVvynnLi2nsb2yYQ-4PmE3zzcj4yQkk5rJyA/edit?usp=sharing
