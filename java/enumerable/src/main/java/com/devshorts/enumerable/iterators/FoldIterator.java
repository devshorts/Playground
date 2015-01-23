package com.devshorts.enumerable.iterators;

import java.util.function.BiFunction;

public class FoldIterator<TSource, TAcc> extends MapIterator<TSource, TAcc> {
    private final BiFunction<TAcc, TSource, TAcc> accumulator;
    private TAcc seed;


    public FoldIterator(Iterable input,
                        BiFunction<TAcc, TSource, TAcc> accumulator,
                        TAcc seed) {
        super(input);
        this.accumulator = accumulator;
        this.seed = seed;
    }

    @Override
    public boolean hasNext(){
        return true;
    }

    @Override
    public TAcc next(){
        while(source.hasNext()){
            seed = accumulator.apply(seed, (TSource)source.next());
        }

        return seed;
    }
}
