# module Relativity
#     module Helper
  
#       include Chef::Mixin::ShellOut
  
#       def execute_code_with_retry(code, max_retry, cmd_returns_value)
#         retry_cnt = 0
#         maximum_retry = max_retry
#         result = nil
#         error  = nil

#         while retry_cnt < maximum_retry && result.nil?
#             begin
#                 result = eval(code) 
#             rescue Exception => e
#                 error = e.to_s
#             end
#             retry_cnt += 1
#           end

#         if result and cmd_returns_value == true
#             result
#         elseif error
#             raise error
#         end
        
#       end
#     end
#   end


class Chef::Recipe::RetryHelper
  
      include Chef::Mixin::ShellOut
  
      def self.execute_code_with_retry(code, max_retry, cmd_returns_value)
        retry_cnt = 0
        maximum_retry = max_retry
        result = nil
        error  = nil

        while retry_cnt == 0 or (retry_cnt < maximum_retry && result.nil? && !error.nil?)
            begin
                puts "begin start"
                result = eval(code) 
                puts "results is nil = #{result.nil?}"
            rescue Exception => e
                puts "rescue start"
                error = e.to_s
                puts error
            end
            retry_cnt += 1
          end

        if result and cmd_returns_value == true
            puts result
        elsif error
            raise error
        end
        
      end

      def self.execute_rest_call(http_obj, request, max_retry, err_msg)
        retry_cnt = 0
        maximum_retry = max_retry
        result = nil
        error  = nil

        while retry_cnt == 0 or (retry_cnt < maximum_retry && result.nil? && !error.nil?)
            begin
                result = http_obj.request(request)
                if !result.is_a? Net::HTTPSuccess
                    raise err_msg +" " +result.message + " " +result.body
                end
            rescue Exception => e
                error = e.to_s
            end
            retry_cnt += 1
          end

        if result
            result
        elsif error
            raise error
        end
        
      end
  end