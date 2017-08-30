#pragma config(Motor,port3,MOTOR_right_drive,tmotorVex393_HBridge,openLoop)
#pragma config(Motor,port10,MOTOR_front_left_drive,tmotorVex393_HBridge,openLoop,reversed)
#pragma config(Motor,port1,MOTOR_back_left_drive,tmotorVex393_HBridge,openLoop)
#pragma config(Motor,port5,MOTOR_right_arm_one,tmotorVex393_HBridge,openLoop)
#pragma config(Motor,port4,MOTOR_right_arm_two,tmotorVex393_HBridge,openLoop)
#pragma config(Motor,port7,MOTOR_left_arm_one,tmotorVex393_HBridge,openLoop,reversed)
#pragma config(Motor,port6,MOTOR_left_arm_two,tmotorVex393_HBridge,openLoop,reversed)
#pragma config(Motor,port9,MOTOR_claw_left,tmotorVex393_HBridge,openLoop,reversed)
#pragma config(Motor,port8,MOTOR_claw_right,tmotorVex393_HBridge,openLoop)
/* Created using CodeBot2 - https://github.com/wg4568/CodeBot2 */
float CONST_arm_speed = 120;
float CONST_hover_speed = 10;
float CONST_claw_speed = 50;
float INPUT_OLD_claw_close = 0.0;
float INPUT_claw_close = 0.0;
float INPUT_OLD_claw_open = 0.0;
float INPUT_claw_open = 0.0;
float INPUT_OLD_arm_up = 0.0;
float INPUT_arm_up = 0.0;
float INPUT_OLD_arm_hover = 0.0;
float INPUT_arm_hover = 0.0;
float INPUT_OLD_arm_down = 0.0;
float INPUT_arm_down = 0.0;
float INPUT_OLD_arm_backhover = 0.0;
float INPUT_arm_backhover = 0.0;
float INPUT_OLD_left_input = 0.0;
float INPUT_left_input = 0.0;
float INPUT_OLD_right_input = 0.0;
float INPUT_right_input = 0.0;
void TRIGGER_always_left_input() {
if (true) {
motor[MOTOR_front_left_drive] = INPUT_left_input;
motor[MOTOR_back_left_drive] = INPUT_left_input;
}
}
void TRIGGER_always_right_input() {
if (true) {
motor[MOTOR_right_drive] = INPUT_right_input;
}
}
void TRIGGER_held_claw_close() {
if (INPUT_OLD_claw_close == 1) {
motor[MOTOR_claw_left] = CONST_claw_speed;
motor[MOTOR_claw_right] = CONST_claw_speed;
}
}
void TRIGGER_held_claw_open() {
if (INPUT_OLD_claw_open == 1) {
motor[MOTOR_claw_left] = -CONST_claw_speed;
motor[MOTOR_claw_right] = -CONST_claw_speed;
}
}
void TRIGGER_up_claw_close() {
if (INPUT_OLD_claw_close == 1 && INPUT_claw_close == 0) {
motor[MOTOR_claw_left] = 0;
motor[MOTOR_claw_right] = 0;
}
}
void TRIGGER_up_claw_open() {
if (INPUT_OLD_claw_open == 1 && INPUT_claw_open == 0) {
motor[MOTOR_claw_left] = 0;
motor[MOTOR_claw_right] = 0;
}
}
void TRIGGER_held_arm_up() {
if (INPUT_OLD_arm_up == 1) {
motor[MOTOR_right_arm_one] = CONST_arm_speed;
motor[MOTOR_right_arm_two] = CONST_arm_speed;
motor[MOTOR_left_arm_one] = CONST_arm_speed;
motor[MOTOR_left_arm_two] = CONST_arm_speed;
}
}
void TRIGGER_held_arm_down() {
if (INPUT_OLD_arm_down == 1) {
motor[MOTOR_right_arm_one] = -CONST_arm_speed;
motor[MOTOR_right_arm_two] = -CONST_arm_speed;
motor[MOTOR_left_arm_one] = -CONST_arm_speed;
motor[MOTOR_left_arm_two] = -CONST_arm_speed;
}
}
void TRIGGER_held_arm_hover() {
if (INPUT_OLD_arm_hover == 1) {
motor[MOTOR_right_arm_one] = CONST_hover_speed;
motor[MOTOR_right_arm_two] = CONST_hover_speed;
motor[MOTOR_left_arm_one] = CONST_hover_speed;
motor[MOTOR_left_arm_two] = CONST_hover_speed;
}
}
void TRIGGER_up_arm_up() {
if (INPUT_OLD_arm_up == 1 && INPUT_arm_up == 0) {
motor[MOTOR_right_arm_one] = 0;
motor[MOTOR_right_arm_two] = 0;
motor[MOTOR_left_arm_one] = 0;
motor[MOTOR_left_arm_two] = 0;
}
}
void TRIGGER_up_arm_down() {
if (INPUT_OLD_arm_down == 1 && INPUT_arm_down == 0) {
motor[MOTOR_right_arm_one] = 0;
motor[MOTOR_right_arm_two] = 0;
motor[MOTOR_left_arm_one] = 0;
motor[MOTOR_left_arm_two] = 0;
}
}
void TRIGGER_up_arm_hover() {
if (INPUT_OLD_arm_hover == 1 && INPUT_arm_hover == 0) {
motor[MOTOR_right_arm_one] = 0;
motor[MOTOR_right_arm_two] = 0;
motor[MOTOR_left_arm_one] = 0;
motor[MOTOR_left_arm_two] = 0;
}
}
task main() { while (true) {
INPUT_claw_close = vexRT[Btn5D];
INPUT_claw_open = vexRT[Btn5U];
INPUT_arm_up = vexRT[Btn6U];
INPUT_arm_hover = vexRT[Btn6D];
INPUT_arm_down = vexRT[Btn8R];
INPUT_arm_backhover = vexRT[Btn8D];
INPUT_left_input = vexRT[Ch3];
INPUT_right_input = vexRT[Ch2];
TRIGGER_always_left_input();
TRIGGER_always_right_input();
TRIGGER_held_claw_close();
TRIGGER_held_claw_open();
TRIGGER_up_claw_close();
TRIGGER_up_claw_open();
TRIGGER_held_arm_up();
TRIGGER_held_arm_down();
TRIGGER_held_arm_hover();
TRIGGER_up_arm_up();
TRIGGER_up_arm_down();
TRIGGER_up_arm_hover();
INPUT_OLD_claw_close = INPUT_claw_close;
INPUT_OLD_claw_open = INPUT_claw_open;
INPUT_OLD_arm_up = INPUT_arm_up;
INPUT_OLD_arm_hover = INPUT_arm_hover;
INPUT_OLD_arm_down = INPUT_arm_down;
INPUT_OLD_arm_backhover = INPUT_arm_backhover;
INPUT_OLD_left_input = INPUT_left_input;
INPUT_OLD_right_input = INPUT_right_input;
}}
