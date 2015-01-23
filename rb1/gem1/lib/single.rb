require 'json'

class Hash
  def on_key(key)
    if has_key? key
      yield self[key]
    end
  end
end

module Ones

  class Config
    def initialize(str)
      JSON.parse(str)
    end

  end
  class State

    def initialize
      @curr = :begin
    end

    def step
      state_order.on_key(@curr) do | val |
        @curr = state_order[val]
        send(@curr)
      end
    end

    def back
      state_order_back.on_key(@curr) do | val |
        @curr = val
        send(@curr)
      end
    end

    private

    def state_order
      {
          :begin => :connect,
          :connect => :start,
          :start => :run,
          :run => :close,
          :close => :close
      }
    end

    def state_order_back
      state_order.invert
    end

    def begin

    end
    def connect
      puts 'connect'
    end

    def start
      puts 'start'
    end

    def run
      puts 'run'
    end

    def close
      puts 'close'
    end
  end
end

