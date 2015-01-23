package com.devshorts.enumerable.iterators;

import java.util.Iterator;
import java.util.function.BiFunction;

public class ZipIterator<T1, T2, T3> extends MapIterator<T1, T3> {

    private Iterator<T2> zipWithIterator;
    private BiFunction<T1, T2, T3> zipper;

    public ZipIterator(Iterable<T1> input, Iterable<T2> zipWith, BiFunction<T1, T2, T3> zipper) {
        super(input);
        this.zipWithIterator = zipWith.iterator();
        this.zipper = zipper;
    }

    @Override
    public boolean hasNext(){
        return source.hasNext() && zipWithIterator.hasNext();
    }

    @Override
    public T3 next(){
        T1 n1 = (T1)source.next();
        T2 n2 = zipWithIterator.next();

        return zipper.apply(n1, n2);
    }
}
