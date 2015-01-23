package com.devshorts.enumerable.data;

enum YieldType{
    YIELD,
    BREAK
}
public class Yieldable<T> {
    public static <T> Yieldable<T> yield(T element){
        return new Yieldable<>(YieldType.YIELD, element);
    }

    public static <T> Yieldable<T> yieldBreak(){
        return new Yieldable<>(YieldType.BREAK, null);
    }

    private Yieldable(YieldType type, T element){
        yieldType = type;
        this.element = element;
    }

    private YieldType yieldType;
    private T element;

    public T value(){
        return element;
    }

    public boolean isYield(){
        return yieldType == YieldType.YIELD;
    }

    public boolean isBreak(){
        return yieldType == YieldType.BREAK;
    }
}
