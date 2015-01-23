package com.devshorts.enumerable.iterators;

import java.util.function.BiFunction;

public class FoldWithDefaultSeedIterator<TSource, TAcc> extends MapIterator<TSource, TAcc> {
    private final BiFunction<TAcc, TSource, TAcc> accumulator;
    private TAcc seed;

    public FoldWithDefaultSeedIterator(Iterable input,
                                       BiFunction<TAcc, TSource, TAcc> accumulator) {
        super(input);
        this.accumulator = accumulator;
    }

    @Override
    public boolean hasNext(){
        return source.hasNext();
    }

    @Override
    public TAcc next(){
        if(seed == null){
            seed = source.next();
        }

        while(source.hasNext()){
            seed = accumulator.apply(seed, (TSource)source.next());
        }

        return seed;
    }
}
