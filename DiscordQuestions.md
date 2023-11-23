To get a general idea on algebraic effects, I used this blog post: https://overreacted.io/algebraic-effects-for-the-rest-of-us/

Then I mapped out the control flow from one of the examples within.

As you can see, control leaves the `getName()` function to an ambient context in order to perform the `ask_name` effect (this happens in `name = perform 'ask_name'`). Further up the stack, `connect()` has registered a handler for this effect (`resume with 'Arya Stark'`), so that handler will be executed and its result returned to the context. That, in turn, passes the result back to the `getName` function. 

Because handlers can essentially be arbitrary code, I was investigating ways to enable an easy way to define handlers and replace that code with something that accesses the ambiant context instead.

I hope this gives some context to my question.