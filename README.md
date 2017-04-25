# MidnightSun

> This is a story about murder, betrayal
> 
> This is also a story about love
> 
> This is a journey under the midnight sun


*To be continued...*

---

## Demo:

![](./demoImage/20170425.gif)


---

## Task for Snap!

### Task 1

1. 需要实现一个page，page为聊天窗口，聊天对话框用最 low 方框 的就可以了。
2. 用一个数据库存储你和她的所有对话：

id | msg | sender
------|------|----
1 | 你好~ | 0
2 | 我的名字叫做 Yukiho，你可以叫我……雪穗  | 0
3 | 你好，很高兴认识你| 1
4 | 我想给你说说心里话…… | 0
... | ... | ...

3. 页面被调起时，程序将会读入这个数据库里的所有信息，如果 **sender == 0**，聊天窗口显示在左侧（雪穗说的话），如果 **sender == 1**，这句话是“你”说的，聊天窗口显示在右边。默认加载到最下面。



### Task 2, 3, 4, 5...

To be continued...

---


## 剧情存储

*(Demo 不提供数据库，假设如下数据库已经实现的操作)*

---

2017.04.24更新 (先阅读原方法)：

在每一个数据库，记录一个关键词作为next的值，如"choose"，用于判断是否需要调起用户选择选项：

E.g.

**X1.db:**

id | msg | Next
------|------|----
1 | Hello | null
2 | Thank you | null
3 | Are you OK? | "choose"
4 | No, I'm not OK | null
5 | Yes, I'm very Ok | null

让用户选择，0 or 1，读入用户输入，0或1，加载X1之后，如X10, X11，进入下一个。

**X10.db:**

id | msg | Next
------|------|----
1 | You are not ok！ | X2

最后一个元素next值不为空，直接进入 X2.db

**X11.db:**

id | msg | Next
------|------|----
1 | You are ok！ | X2

设数据库中所有的元素为AllItems

```cs

int flag = -1; // (means do not read)
string choose[] = new string[2]; //Record choosing msg

foreach (var item in AllItems) {
    if (flag > -1) {
        choose[flag++] = item.msg;
    }
    if (item.next == "choose") {
        ++flag;
    }
}

```

----

原方法：

每个数据库只有最后一个的元素的Next的值可以是null或者非空。

如果跑完一个数据库：

**1. 最后一个元素的Next的值是null，读取用户输入，然后加在文件名里。**
**2. 最后一个元素的Next不是null，是例如"X2"，则直接读取Next对应值的db文件。**

---

E.g.

**X1.db:**

id | msg | Next
------|------|----
1 | Hello | null
2 | Thank you | null
3 | Are you OK? | null

读入用户输入，0或1，加载X1之后，如X10, X11，进入下一个。

**X11.db:**

id | msg | Next
------|------|----
1 | You are ok！ | X2

**X10.db:**

id | msg | Next
------|------|----
1 | You are not ok！ | X2

最后一个元素next值不为空，直接进入 X2.db

**X2.db:**

id | msg | Next
------|------|----
1 | Do you like MI4-I? |null

同理，读入用户输入，进入X21或X20

**X21.db:**

id | msg | Next
------|------|----
1 | You like MI4-I！ |null
2 | I'm very glad you like！ |X3

**X20.db:**

id | msg | Next
------|------|----
1 | Oh, You don't like MI4-I? |null
2 | You don't like me... |null
3 | Nevermind~ |X3

**X3.db:**

id | msg | Next
------|------|----
1 | I will give everyone a MI-Band! |null


......

