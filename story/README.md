# 剧情存储

*(Demo 不提供数据库，假设如下数据库已经实现的操作)*

---

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
