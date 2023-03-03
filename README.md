
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

Our develop will follow the workflow "Gitlow"

We will have **a master branch, a develop branch, several hotfixs, release, fearture branches**

1. Only hotfixes and release branch can merge to master.
2. hotfixes only fix bugs from master
3. develop must first merge to release and fix bugs there
4. feature branches will merge to develop branch

![img](https://lh5.googleusercontent.com/lQi1fsL5G88MPd9kZRjXGGDb-pUqQW0aVIA4VrP9cgfjuy8j7a7cnQ1_7nW5dZrH0-QJCA-SOkKDq8utCZwvrY8KwHOvZWUsj44oSLP3AZ_sLSutTTWwNyp6WSPRNcUGd23r95kIz8xLNyaa7TdpA5xzBq2fRMPI_HCdwsGYHhYvyzb7T70a9GjoLRcsyw)

---
## Presentation

Every team will present in a **15 minute presentation** what they did in the lab. Use the notes from your Google document(s) to create an overview of your steps, what you did in your weekly sessions and how the final version of your work looks like. 



2. Introduction to the systems your team worked on. Make sure the audience understands the motivation behind them, why your team built them and what they could be useful for
2. Please include the videos of the different **iterations** you did in the presentation.
      1. Detection: 对于预导入的图片检测并显示一个方块在这之上
      2. Live capture and detection: 加入图片capture功能并实时加入图片库，并可以检测出来
      3. Image save: 第二次迭代中capture的图片会被保存，并在下次打开app的时候识别
      4. Mode distinction：将上代的功能分割到 Capture和Detection两个mode，并实现mode切换
      5. CaptureMode: 加入地点输入，图片以特别的名称保存 **[LocationName]UniqueIdentifier**
      6. DetectionMode:通过正则语言识别地点名，在连续检测到三个相同Location的图片的时候返回Location。识别之后在画面上显示绿色Detected
      7. RefineUI: 添加美化UI
	  8. Save/Load Image Location/Position: 图片以特别的名称保存位置 **[LocationName]{Position}\`Rotation\`UniqueIdentifier**
	  9. **(!Disposed)** Add AR plain detection, tap the screen to place a cube, store/load the location of the cube in the form of json
	  10. Offset calculation: calculate the offset, place the cube again in currenet scene with regards to the offset.
3. The most interesting details about your implementation efforts and learnings about the system you worked with:  Extract the most interesting details about your work and explain to the other students why you think it's worth mentioning Your slides should not all be walls of code, visual content such as conceptual diagrams and recordings of demos often work better to explain concepts & algorithms
      1. Gitflow
4. The results: The output of your team's work to show what it can do in its final stage. Make sure the audience would understand what they would get if they would use your component.
5. A slide about where your team had problems in the lab
      1. Android Version
6. A slide about what did work well in the lab
7. A summary & Questions slide

