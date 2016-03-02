# Custom Programming Language Compiler

Compiler of very basic custom language that uses recursive descent parser and is written in C#.

FASM.EXE with Include folder needs to be put in the same directory as compiled project
in order to allow compiler to create executable files.

**CODE EXAMPLES:**

1.
```
void main()
{
  string str = "Hello World"
  WriteString(str)
};
```
2.
```
num square(num b)
{
  num a = b*b;
  return a;
};

void main()
{
  num a = square(2);
  WriteNum(a);
};
```
3.
```
void main()
{
  num a;
  num b;
  string s1 = "first number";
  string s2 ="second number";
  string s3 = "numbers are equal";
  string s4 ="numbers are not equal";
  WriteString(s1);
  a = ReadNum();
  WriteString(s2);
  b = ReadNum();
  if a==b;
    WriteString(s3);
  else;
    WriteString(s4);
  endif;
};
```
